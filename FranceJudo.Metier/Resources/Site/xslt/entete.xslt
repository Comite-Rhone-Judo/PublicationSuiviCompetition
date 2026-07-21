<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="entete">
		<xsl:param name="logo"/>
		<xsl:param name="logoDark"/>
		<!-- Nouveau paramètre pour le logo inversé -->
		<xsl:param name="affProchainCombats"/>
		<xsl:param name="affAffectationTapis"/>
		<xsl:param name="affEngagements"/>
		<xsl:param name="affStatistiques"/>
		<xsl:param name="affActualiser"/>
		<xsl:param name="selectedItem"/>
		<xsl:param name="pathToImg"/>
		<xsl:param name="pathToCommon"/>
		<xsl:variable name="apos">'</xsl:variable>

		<!-- BANDEAU DE TITRE -->
		<div class="tas-titre-app">
			<div class="w3-cell-row">
				<button class="w3-button w3-transparent tas-adaptive-icon w3-large tas-margin-none" onclick="openElement('navigationPanel')">☰</button>
				<h3>Suivi compétition</h3>
				<div class="bandeau-titre">
					<!-- Logo principal (Classe dynamique selon la présence du logo sombre) -->
					<img>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$logoDark != ''">img-bandeau-titre tas-logo-light</xsl:when>
								<xsl:otherwise>img-bandeau-titre tas-logo-auto-invert</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:attribute name="src">
							<xsl:value-of select="concat($pathToImg, $logo)"/>
						</xsl:attribute>
					</img>

					<!-- Logo Mode Sombre spécifique (si fourni) -->
					<xsl:if test="$logoDark != ''">
						<img class="img-bandeau-titre tas-logo-dark">
							<xsl:attribute name="src">
								<xsl:value-of select="concat($pathToImg, $logoDark)"/>
							</xsl:attribute>
						</img>
					</xsl:if>
				</div>
			</div>
		</div>

		<!-- PANNEAU DE NAVIGATION -->
		<div class="w3-sidebar w3-animate-left tas-navigation-panel" id="navigationPanel">
			<div class="tas-nav-flex-container">

				<!-- Bouton Fermer (à droite, sans ligne en dessous) -->
				<button onclick="closeElement('navigationPanel')" class="w3-large tas-adaptive-icon tas-nav-close-btn">Fermer &times;</button>

				<!-- Actualiser (sans ligne au dessus, mais avec ligne en dessous) -->
				<xsl:if test="$affActualiser">
					<div class="tas-nav-toggle-item tas-switch-container">
						<label class="tas-switch">
							<input type="checkbox" id="cbActualiser" onclick="toggleAutoRefresh(this);"/>
							<span class="tas-slider"></span>
						</label>
						<span class="tas-switch-label">Actualiser</span>
					</div>
				</xsl:if>

				<xsl:if test="$affProchainCombats">
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="concat($pathToCommon, 'se_prepare.html')"/>
						</xsl:attribute>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$selectedItem = 'se_prepare'">navButton nav-active</xsl:when>
								<xsl:otherwise>navButton</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						Se prépare
					</a>
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="concat($pathToCommon, 'prochains_combats.html')"/>
						</xsl:attribute>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$selectedItem = 'prochains_combats'">navButton nav-active</xsl:when>
								<xsl:otherwise>navButton</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						Prochains combats
					</a>
				</xsl:if>

				<xsl:if test="$affAffectationTapis">
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="concat($pathToCommon, 'affectation_tapis.html')"/>
						</xsl:attribute>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$selectedItem = 'affectations_tapis'">navButton nav-active</xsl:when>
								<xsl:otherwise>navButton</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						Affectations
					</a>
				</xsl:if>

				<a>
					<xsl:attribute name="href">
						<xsl:value-of select="concat($pathToCommon, 'avancement.html')"/>
					</xsl:attribute>
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$selectedItem = 'avancement'">navButton nav-active</xsl:when>
							<xsl:otherwise>navButton</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					Avancements
				</a>

				<a>
					<xsl:attribute name="href">
						<xsl:value-of select="concat($pathToCommon, 'classement.html')"/>
					</xsl:attribute>
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$selectedItem = 'classement'">navButton nav-active</xsl:when>
							<xsl:otherwise>navButton</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					Classements
				</a>

				<xsl:if test="$affEngagements">
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="concat($pathToCommon, 'engagements.html')"/>
						</xsl:attribute>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$selectedItem = 'engagements'">navButton nav-active</xsl:when>
								<xsl:otherwise>navButton</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						Engagements
					</a>
				</xsl:if>

				<xsl:if test="$affStatistiques">
					<a>
						<xsl:attribute name="href">
							<xsl:value-of select="concat($pathToCommon, 'statistiques.html')"/>
						</xsl:attribute>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$selectedItem = 'statistiques'">navButton nav-active</xsl:when>
								<xsl:otherwise>navButton</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						Statistiques
					</a>
				</xsl:if>

				<!-- Footer repoussé en bas par le flex -->
				<div class="tas-nav-footer">
					<div class="tas-switch-container">
						<label class="tas-switch">
							<input type="checkbox" id="cbDarkMode" onclick="toggleDarkMode(this);"/>
							<!-- CORRECTION : L'ajout de texte empêche le XSLT de casser la balise -->
							<span class="tas-slider">
								<xsl:text> </xsl:text>
							</span>
						</label>
						<span class="tas-switch-label">Mode sombre</span>
					</div>
				</div>

			</div>
			<!-- Fin du conteneur Flex -->
		</div>
		<!-- Fin du panneau de navigation -->
	</xsl:template>
</xsl:stylesheet>