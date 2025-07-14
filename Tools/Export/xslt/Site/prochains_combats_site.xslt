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
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>


	<xsl:key name="combats" match="combat" use="@niveau"/>
	<xsl:template match="/">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable select="count(/competitions/competition[@PublierProchainsCombats = 'true']) > 0" name="affProchainCombats"/>
	<xsl:variable select="count(/competitions/competition[@PublierAffectationTapis = 'true']) > 0" name="affAffectationTapis"/>
	<xsl:variable select="count(/competitions/competition[@PublierEngagements = 'true']) > 0" name="affEngagements"/>
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
			<link type="text/css" rel="stylesheet">
				<xsl:attribute name="href">
					<xsl:value-of select="concat($cssPath, 'w3.css')"/>
				</xsl:attribute>
			</link>
			<link type="text/css" rel="stylesheet">
				<xsl:attribute name="href">
					<xsl:value-of select="concat($cssPath, 'style-common.css')"/>
				</xsl:attribute>
			</link>

			<!-- Script de navigation par defaut -->
			<script>
				<xsl:attribute name="src">
					<xsl:value-of select="concat($jsPath, 'site-display.js')"/>
				</xsl:attribute>
			</script>

			<!-- Script ajoute en parametre -->
			<script type="text/javascript">
				<xsl:value-of select="$js"/>
			</script>
			<title>
				Suivi Compétition - Prochains Combats
			</title>
		</head>
		<body>
			<!-- ENTETE -->
			<xsl:call-template name="entete">
				<xsl:with-param name="logo" select="$logo"/>
				<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
				<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
				<xsl:with-param name="affEngagements" select="$affEngagements"/>
				<xsl:with-param name="affActualiser" select="false()"/>
				<xsl:with-param name="selectedItem" select="'prochains_combats'"/>
				<xsl:with-param name="pathToImg" select="$imgPath"/>
				<xsl:with-param name="pathToCommon" select="$commonPath"/>
			</xsl:call-template>

			<!-- Div vide pour aligner le contenu avec le bandeau de titre de taille fixe -->
			<div class="w3-container tas-filler-div">&nbsp;</div>

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
					<script>
						<xsl:attribute name="src">
							<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
						</xsl:attribute>
					</script>
				</div>
			</xsl:if>

		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Un bloc -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>
		<xsl:variable name="prefixCompetition">
			<xsl:value-of select="concat('ProchainCombatComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>

		<!-- Nom de la competition -->
		<div class="w3-container w3-blue w3-center tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Avancements" class="w3-container w3-border pane w3-animate-left">
			<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
			<div class="w3-cell-row">
				<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
				<!-- Categorie F -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'F'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<!-- Categorie M -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'M'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<!-- Mixte -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'X'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
			</div>
		</div>

	</xsl:template>

	<!-- Bouton avancement par epreuve -->
	<!-- On en tient compte que des epreuves pour lesquelles les phases sont crees et sans classement valide -->
	<xsl:template name="prochains_combats_epreuve" match="epreuve">
		<xsl:if test="count(./phases/phase[number(@typePhase) = 1 and number(@etat) > 0 and number(@etat) != 5]) > 0">
			<a class="w3-button w3-panel w3-card w3-block w3-pale-yellow w3-large w3-round-large w3-padding-small">
				<xsl:attribute name="href">
					<xsl:value-of select="concat($competitionPath, @directory, '/feuille_combats.html')"/>
				</xsl:attribute>
				<xsl:value-of select="./@libelle"/>	
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Poules</xsl:text>
			</a>
		</xsl:if>
		<xsl:if test="count(./phases/phase[number(@typePhase) = 2 and number(@etat) > 0 and number(@etat) != 5]) > 0">
			<a class="w3-button w3-panel w3-card w3-block w3-pale-yellow w3-large w3-round-large w3-padding-small">
				<xsl:attribute name="href">
					<xsl:value-of select="concat($competitionPath, @directory, '/feuille_combats.html')"/>
				</xsl:attribute>
				<xsl:value-of select="./@libelle"/>
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Tableau</xsl:text>
			</a>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
