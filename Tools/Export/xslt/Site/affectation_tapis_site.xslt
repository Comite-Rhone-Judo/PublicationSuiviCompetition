<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>
	<xsl:import href="Tools/Export/xslt/Site/panel_epreuve.xslt"/>

	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	

	<xsl:key name="combats" match="combat" use="@niveau"/>
	<xsl:template match="/">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable select="count(/competitions/competition[@PublierProchainsCombats = 'True']) > 0" name="affProchainCombats"/>
	<xsl:variable select="count(/competitions/competition[@PublierAffectationTapis = 'True']) > 0" name="affAffectationTapis"/>
	<xsl:variable select="sum(/competitions/competition/@DelaiActualisationClientSec) div count(/competitions/competition)" name="delayActualisationClient"/>
	<xsl:variable select="/competitions/competition[1]/@Logo" name="logo"/>

	<xsl:template match="/*">
		<!-- ENTETE HTML -->
		<head>
			<META http-equiv="Content-Type" content="text/html; charset=utf-8"/>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
			<meta name="viewport" content="width=device-width,initial-scale=1"/>
			<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
			<meta http-equiv="Pragma" content="no-cache"/>
			<meta http-equiv="Expires" content="0"/>

			<!-- Feuille de style W3.CSS -->
			<link type="text/css" rel="stylesheet" href="../style/w3.css"/>
			<link type="text/css" rel="stylesheet" href="../style/style-common.css"/>

			<!-- Script de navigation par defaut -->
			<script src="../js/site-display.js"/>

			<!-- Script ajoute en parametre -->
			<script type="text/javascript">
				<xsl:value-of select="$js"/>
				var delayAutoreloadSec = <xsl:value-of select="$delayActualisationClient"/>;
				window.onload=checkReloading;
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<!-- ENTETE -->
			<xsl:call-template name="entete">
				<xsl:with-param name="logo" select="$logo"/>
				<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
				<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
				<xsl:with-param name="affActualiser" select="'True'"/>
				<xsl:with-param name="selectedItem" select="'affectations_tapis'"/>
			</xsl:call-template>
			
			<!-- CONTENU -->
			<xsl:if test="count(/competitions/competition)=0 or count(//epreuve)=0">
				<div class="w3-container w3-border">
					<div class="w3-panel w3-pale-green w3-bottombar w3-border-green w3-border w3-center w3-large"> Veuillez patienter le tirage des épreuves </div>
				</div>
			</xsl:if>
			
			<!-- Boucle global sur les competitions en cours -->
			<xsl:for-each select="/competitions/competition">
				<xsl:if test="count(./epreuve) > 0">
					<xsl:variable name="compet" select="@ID"/>
					<xsl:call-template name="competition">
						<xsl:with-param name="idcompetition" select="$compet"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>

			<xsl:if test="count(/competitions/competition)>0">
				<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
					<script src="../js/footer_script.js"/> <!-- TODO penser a modifier quand on passera en version Participants -->
				</div>
			</xsl:if>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- Un bloc -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>
		<xsl:variable name="prefixCompetition">
			<xsl:value-of select="concat('AffectationComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>

		<!-- Nom de la competition -->
		<div class="w3-container w3-blue w3-center tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Affectations" class="w3-container w3-border pane w3-animate-left">
			<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
			<div class="w3-cell-row">
				<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
				<!-- Categorie F -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'F'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
				</xsl:call-template>
				<!-- Categorie M -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'M'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
				</xsl:call-template>
				<!-- Mixte -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'X'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
				</xsl:call-template>
			</div>
		</div>
	</xsl:template>
	
	<!-- Bouton affectations par epreuve -->
	<xsl:template name="affectation_epreuve" match="epreuve">
		<div class="w3-panel">
			<div class="w3-card">
				<header class="w3-container w3-pale-yellow w3-large w3-padding-small">
					<xsl:value-of select="./@libelle"/>
					<xsl:value-of select="./@nom"/>
				</header>
				<div class="w3-container w3-cell w3-cell-middle w3-padding">
					<xsl:choose>
						<xsl:when test="count(./TapisEpreuve/tapis) > 1">
							<xsl:text>Tapis&#32;</xsl:text>
							<xsl:for-each select="./TapisEpreuve/tapis">
								<xsl:sort select="./@no_tapis" data-type="number"/>
								<xsl:value-of select="./@no_tapis"/>
								<xsl:if test="position() &lt; last() - 1">
									<xsl:text>, </xsl:text>
								</xsl:if>
								<xsl:if test="position() = last() - 1">
									<xsl:text> et </xsl:text>
								</xsl:if>
							</xsl:for-each>
						</xsl:when>
						<xsl:when test="count(./TapisEpreuve/tapis) = 1">
							<xsl:text>Tapis&#32;</xsl:text>
							<xsl:for-each select="./TapisEpreuve/tapis">
								<xsl:sort select="./@no_tapis" data-type="number"/>
								<xsl:value-of select="./@no_tapis"/>
							</xsl:for-each>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>&nbsp;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</div>
		</div>
	</xsl:template>

</xsl:stylesheet>