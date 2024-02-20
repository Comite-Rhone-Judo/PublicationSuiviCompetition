<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="entete">
		<xsl:param name="logo"/>
		<xsl:param name="affProchainCombats"/>
		<xsl:param name="affAffectationTapis"/>
		<xsl:param name="affActualiser"/>
		<xsl:param name="selectedItem"/>
		<xsl:variable name="apos">'</xsl:variable>
		
		<!-- BANDEAU DE TITRE -->
		<div class="w3-top tas-titre-app">
			<div class="w3-cell-row w3-light-grey">
				<button class="w3-cell w3-button w3-xlarge w3-cell-left" onclick="openElement('navigationPanel')">☰</button>
				<div class="w3-cell w3-cell-middle w3-center">
					<h3>Suivi compétition</h3>
				</div>
				<div class="w3-cell w3-cell-middle bandeau-titre">
					<img class="img img-bandeau-titre">
						<xsl:attribute name="src">
							<xsl:value-of select="concat('../img/',$logo)"/>
						</xsl:attribute>
					</img>
				</div>
			</div>
		</div>

		<!-- PANNEAU DE NAVIGATION -->
		<div class="w3-sidebar w3-bar-block w3-border-right w3-animate-left tas-navigation-panel" id="navigationPanel">
			<button onclick="closeElement('navigationPanel')" class="w3-bar-item w3-large">Fermer &times;</button>
			<xsl:if test="$affActualiser or $affActualiser = 'True'">
				<button class="w3-bar-item w3-button navButton">
					<input class="w3-check" type="checkbox" id="cbActualiser" onclick="toggleAutoRefresh(this);"/> Actualiser
				</button>
			</xsl:if>
			<xsl:if test="$affProchainCombats   or $affProchainCombats = 'True'">
				<a href="../common/se_prepare.html">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$selectedItem = 'se_prepare'">w3-bar-item w3-button navButton</xsl:when>
							<xsl:otherwise>w3-bar-item w3-button navButton w3-indigo</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					Se prépare
				</a>
				<a href="../common/prochains_combats.html">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$selectedItem = 'prochains_combats'">w3-bar-item w3-button navButton</xsl:when>
							<xsl:otherwise>w3-bar-item w3-button navButton w3-indigo</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					Prochains combats
				</a>
			</xsl:if>
			<xsl:if test="$affAffectationTapis  or $affAffectationTapis = 'True'">
				<a href="../common/affectation_tapis.html">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$selectedItem = 'affectations_tapis'">w3-bar-item w3-button navButton</xsl:when>
							<xsl:otherwise>w3-bar-item w3-button navButton w3-indigo</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					Affectations
				</a>
			</xsl:if>
			<a href="../common/avancement.html">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$selectedItem = 'avancement'">w3-bar-item w3-button navButton</xsl:when>
						<xsl:otherwise>w3-bar-item w3-button navButton w3-indigo</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				Avancements
			</a>
			<a href="../common/classement.html">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$selectedItem = 'classement'">w3-bar-item w3-button navButton</xsl:when>
						<xsl:otherwise>w3-bar-item w3-button navButton w3-indigo</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				Classements
			</a>
		</div>

		<!-- Div vide pour aligner le contenu avec le bandeau de titre de taille fixe -->
		<div class="w3-container tas-filler-div">&nbsp;</div>
	</xsl:template>
</xsl:stylesheet>
