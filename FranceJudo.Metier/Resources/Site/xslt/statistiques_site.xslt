<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://example.com/2008/data">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/panel_epreuve.xslt"/>

	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="RefData"/>
	<xsl:param name="SiteRoutes"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:template match="docroot">
		<xsl:variable name="apos">'</xsl:variable>
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<META http-equiv="Content-Type" content="text/html; charset=utf-8"/>
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'w3.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-common.css')}"/>

				<script src="{concat($jsPath, 'site-display.js')}"></script>
				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gUseAutoReload = false;
				</script>
				<title>Suivi Compétition - Statistiques</title>
			</head>
			<body>
				<!-- ENTETE -->
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
					<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
					<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
					<xsl:with-param name="affStatistiques" select="true()"/>
					<xsl:with-param name="affEngagements" select="$affEngagements"/>
					<xsl:with-param name="affActualiser" select="false()"/>
					<xsl:with-param name="selectedItem" select="'statistiques'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- CONTENU -->
				<xsl:if test="count(competitions/competition)=0 or count(//groupeStatistiques)=0">
					<div class="w3-padding">
						<div class="ios-card tas-empty-state">Les statistiques ne sont pas encore disponibles</div>
					</div>
				</xsl:if>

				<!-- Boucle globale sur les compétitions en cours -->
				<xsl:for-each select="competitions/competition">
					<xsl:if test="count(./groupesStatistiques/groupeStatistiques) > 0">
						<xsl:call-template name="competition"/>
					</xsl:if>
				</xsl:for-each>

				<xsl:if test="count(competitions/competition)>0">
					<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
						<script src="{concat($jsPath, 'footer_script.js')}"></script>
					</div>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<!-- Template pour le groupement (LA GRILLE EST CORRIGÉE ICI) -->
	<xsl:template name="competition" match="competition">
		<xsl:variable name="idcompetition" select="@ID"/>

		<div class="tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<!-- Remplacement de w3-cell-row par w3-row-padding pour un vrai comportement de grille responsive -->
		<div class="w3-container pane w3-animate-left tas-competition-panels">
			<div class="w3-row-padding">

				<xsl:if test="count(./groupesStatistiques/groupeStatistiques[@sexe = 'F']) > 0">
					<xsl:call-template name="UneCategorie">
						<xsl:with-param name="categorie" select="'F'"/>
					</xsl:call-template>
				</xsl:if>

				<xsl:if test="count(./groupesStatistiques/groupeStatistiques[@sexe = 'M']) > 0">
					<xsl:call-template name="UneCategorie">
						<xsl:with-param name="categorie" select="'M'"/>
					</xsl:call-template>
				</xsl:if>

				<xsl:if test="count(./groupesStatistiques/groupeStatistiques[@sexe = 'X']) > 0">
					<xsl:call-template name="UneCategorie">
						<xsl:with-param name="categorie" select="'X'"/>
					</xsl:call-template>
				</xsl:if>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE CATEGORIE -->
	<xsl:template name="UneCategorie" match="competition">
		<xsl:param name="categorie"/>

		<xsl:variable name="idcompetition" select="@ID"/>
		<xsl:variable name="niveauCompetition" select="@niveau"/>

		<!-- 1. Définition UNIQUE de l'ID du panneau et du préfixe -->
		<xsl:variable name="prefixPanel" select="concat('StatistiquesComp', $idcompetition, 'ContentPanel')"/>
		<xsl:variable name="panelId" select="concat($prefixPanel, $categorie)"/>
		<xsl:variable name="groupTab" select="concat('groupStatComp', $idcompetition, $categorie)"/>

		<!-- Remplacement de w3-cell par w3-col avec des largeurs définies (l4 = 33%, m6 = 50%, s12 = 100%) -->
		<div class="w3-col l4 m6 s12 w3-margin-bottom w3-padding-small">

			<!-- 2. Appel du composant avec la variable -->
			<xsl:call-template name="AccordionButton">
				<xsl:with-param name="sexeCode" select="$categorie"/>
				<xsl:with-param name="targetId" select="$panelId"/>
				<xsl:with-param name="imgPath" select="$imgPath"/>
			</xsl:call-template>

			<!-- 3. Conteneur liant le même ID -->
			<div class="tasClosedPanelType tas-accordion-content-hidden w3-container" id="{$panelId}">
				<xsl:variable name="maxNiveau">
					<xsl:for-each select="//competition[@ID = $idcompetition]/groupesStatistiques">
						<xsl:sort select="@type" data-type="number" order="ascending"/>
						<xsl:if test="position()=last()">
							<xsl:value-of select="@type"/>
						</xsl:if>
					</xsl:for-each>
				</xsl:variable>

				<!-- Contrôle Segmenté iOS -->
				<div class="ios-segmented-control">
					<xsl:for-each select="//competition[@ID = $idcompetition]/groupesStatistiques">
						<xsl:sort order="descending" select="@type"/>
						<xsl:variable name="isActive">
							<xsl:if test="@type = $maxNiveau"> w3-indigo</xsl:if>
						</xsl:variable>

						<xsl:if test="@type = 1">
							<button id="btnNom" data-tabgroup="{$groupTab}" class="ios-segment tasTabBtnType{$isActive}" onclick="openTab('{$groupTab}', 'Nom', true)">Nom</button>
						</xsl:if>
						<xsl:if test="@type = 2">
							<button id="btnClub" data-tabgroup="{$groupTab}" class="ios-segment tasTabBtnType{$isActive}" onclick="openTab('{$groupTab}', 'Club', true)">Club</button>
						</xsl:if>
						<xsl:if test="@type = 3">
							<button id="btnComite" data-tabgroup="{$groupTab}" class="ios-segment tasTabBtnType{$isActive}" onclick="openTab('{$groupTab}', 'Comite', true)">Comité</button>
						</xsl:if>
						<xsl:if test="@type = 4">
							<button id="btnLigue" data-tabgroup="{$groupTab}" class="ios-segment tasTabBtnType{$isActive}" onclick="openTab('{$groupTab}', 'Ligue', true)">Ligue</button>
						</xsl:if>
						<xsl:if test="@type = 5 or @type = 6">
							<button id="btnPays" data-tabgroup="{$groupTab}" class="ios-segment tasTabBtnType{$isActive}" onclick="openTab('{$groupTab}', 'Pays', true)">Pays</button>
						</xsl:if>
					</xsl:for-each>
				</div>

				<!-- Contenu des onglets -->
				<xsl:for-each select="//competition[@ID = $idcompetition]/groupesStatistiques">
					<xsl:sort order="descending" select="@type"/>
					<xsl:variable name="displayStyle">
						<xsl:choose>
							<xsl:when test="@type = $maxNiveau">display: block;</xsl:when>
							<xsl:otherwise>display: none;</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<div class="tasTabType" data-tabgroup="{$groupTab}" style="{$displayStyle}">

						<!-- Niveau Par Nom (Alphabet) -->
						<xsl:if test="@type = 1">
							<xsl:attribute name="id">Nom</xsl:attribute>
							<!-- Grille Flexbox pour éviter de casser la colonne -->
							<div class="ios-alphabet-grid">
								<xsl:apply-templates select="./groupeStatistiques[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="@entite"/>
									<xsl:with-param name="niveauCompetition" select="$niveauCompetition" />
								</xsl:apply-templates>
							</div>
						</xsl:if>

						<!-- Autres Niveaux (Club, Comité, etc.) -->
						<xsl:if test="@type = 2">
							<xsl:attribute name="id">Club</xsl:attribute>
							<xsl:apply-templates select="./groupeStatistiques[@competition = $idcompetition and @sexe = $categorie]">
								<xsl:sort order="ascending" select="$RefData/structures/clubs/club[@ID = current()/@entite]/nom"/>
								<xsl:with-param name="niveauCompetition" select="$niveauCompetition" />
							</xsl:apply-templates>
						</xsl:if>
						<xsl:if test="@type = 3">
							<xsl:attribute name="id">Comite</xsl:attribute>
							<xsl:apply-templates select="./groupeStatistiques[@competition = $idcompetition and @sexe = $categorie]">
								<xsl:sort order="ascending" select="$RefData/structures/comites/comite[@ID = current()/@entite]/nom"/>
								<xsl:with-param name="niveauCompetition" select="$niveauCompetition" />
							</xsl:apply-templates>
						</xsl:if>
						<xsl:if test="@type = 4">
							<xsl:attribute name="id">Ligue</xsl:attribute>
							<xsl:apply-templates select="./groupeStatistiques[@competition = $idcompetition and @sexe = $categorie]">
								<xsl:sort order="ascending" select="$RefData/structures/ligues/ligue[@ID = current()/@entite]/nom"/>
								<xsl:with-param name="niveauCompetition" select="$niveauCompetition" />
							</xsl:apply-templates>
						</xsl:if>
						<xsl:if test="@type = 5 or @type = 6">
							<xsl:attribute name="id">Pays</xsl:attribute>
							<xsl:apply-templates select="./groupeStatistiques[@competition = $idcompetition and @sexe = $categorie]">
								<xsl:sort order="ascending" select="$RefData/structures/lesPays/pays[@ID = current()/@entite]/@nom"/>
								<xsl:with-param name="niveauCompetition" select="$niveauCompetition" />
							</xsl:apply-templates>
						</xsl:if>
					</div>
				</xsl:for-each>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE Bouton groupement (Nettoyé des w3-col défectueux) -->
	<xsl:template name="groupement" match="groupeStatistiques">
		<xsl:param name="niveauCompetition"/>

		<!-- 1. Interrogation du dictionnaire de routage (type: statistique) -->
		<xsl:variable name="urlGroupe" select="$SiteRoutes//routeGroupe[@groupe = current()/@id and @typeGroupe = 'statistique']/@urlGroupe" />
		
		<xsl:variable name="entiteNom">
			<xsl:call-template name="LibelleGroupeStructure">
				<xsl:with-param name="typeGroupe" select="./@type"/>
				<xsl:with-param name="niveauCompetition" select="$niveauCompetition"/>
				<xsl:with-param name="entiteId" select="./@entite"/>
				<xsl:with-param name="RefData" select="$RefData"/>
				<xsl:with-param name="avecPrefixe" select="'false'" />
			</xsl:call-template>
		</xsl:variable>

		<xsl:choose>
			<!-- Pastille circulaire pour les Lettres de l'alphabet (pur Flexbox) -->
			<xsl:when test="./@type = '1'">
				<a class="ios-circle-btn" href="{$urlGroupe}">
					<xsl:value-of select="$entiteNom"/>
				</a>
			</xsl:when>
			<!-- Bouton de liste classique pour les Clubs, Comités, etc. -->
			<xsl:otherwise>
				<a class="ios-list-item" href="{$urlGroupe}">
					<xsl:value-of select="$entiteNom"/>
				</a>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>