<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/panel_epreuve.xslt"/>

	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="SiteRoutes"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable name="imgPath" select="$SiteRoutes/*/@urlImg"/>
	<xsl:variable name="jsPath" select="$SiteRoutes/*/@urlJs"/>
	<xsl:variable name="cssPath" select="$SiteRoutes/*/@urlCss"/>
	<xsl:variable name="commonPath" select="$SiteRoutes/*/@UrlCommon"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<!-- NOUVEAU : Récupération du logo pour le thème sombre -->
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
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

				<!-- Script ajoute en parametre -->
				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
					gDefaultAutoReload = <xsl:value-of select="$actualisationClientDefaut"/>;
				</script>

				<!-- Script de navigation par defaut -->
				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-display.js')"/>
					</xsl:attribute>
				</script>


				<title>
					Suivi Compétition - Affectation
				</title>
			</head>
			<body>
				<!-- ENTETE -->
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
					<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
					<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="affEngagements" select="$affEngagements"/>
					<xsl:with-param name="affStatistiques" select="$affStatistiques"/>
					<xsl:with-param name="selectedItem" select="'affectations_tapis'"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>

				<!-- CONTENU -->
				<xsl:if test="count(competitions/competition)=0 or count(//epreuve)=0">
					<!-- Remplacement par l'Empty State structuré -->
					<div class="w3-padding">
						<div class="ios-card tas-empty-state">
							Veuillez patienter, le tirage des épreuves est en cours...
						</div>
					</div>
				</xsl:if>

				<!-- Boucle global sur les competitions en cours -->
				<xsl:for-each select="competitions/competition">
					<xsl:if test="count(./epreuve) > 0">
						<xsl:variable name="compet" select="@ID"/>
						<xsl:call-template name="competition">
							<xsl:with-param name="idcompetition" select="$compet"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>

				<xsl:if test="count(competitions/competition)>0">
					<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
						<script>
							<xsl:attribute name="src">
								<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
							</xsl:attribute>
						</script>
					</div>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- Un bloc -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>
		<xsl:variable name="prefixCompetition">
			<xsl:value-of select="concat('AffectationComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>

		<!-- Nom de la competition (Nettoyé des couleurs W3.CSS natives) -->
		<div class="tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Affectations" class="w3-container pane w3-animate-left tas-competition-panels">
			<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
			<div class="w3-row-padding">
				<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
				<!-- Categorie F -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'F'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>
				<!-- Categorie M -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'M'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>
				<!-- Mixte -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'X'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>
			</div>
		</div>
	</xsl:template>

	<!-- Bouton affectations par epreuve (Refonte style Carte iOS) -->
	<xsl:template name="affectation_epreuve" match="epreuve">
		<div class="w3-padding-small">
			<div class="ios-card">
				<!-- En-tête avec la classe Flexbox pour aligner le texte et le bouton -->
				<div class="tas-card-header tas-card-header-flex">
					<div>
						<xsl:value-of select="./@libelle"/>
						<xsl:text> </xsl:text>
						<xsl:value-of select="./@nom"/>
					</div>

					<xsl:variable name="idEpreuve" select="@ID" />
					<xsl:variable name="urlAvancement" select="$SiteRoutes//routeEpreuve[@epreuve = $idEpreuve]/@urlAvancement" />
					
					<!-- Le lien est placé UNIQUEMENT autour de l'icône avec sa zone tactile élargie -->
					<xsl:if test="$urlAvancement != ''">
						<a href="{$urlAvancement}" class="tas-icon-btn" title="Voir le tableau">
							<img width="18" src="{$imgPath}tree_structure-32.png" class="tas-theme-icon" alt="Voir l'avancement"/>
						</a>
					</xsl:if>
				</div>
				<div class="tas-card-body">
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