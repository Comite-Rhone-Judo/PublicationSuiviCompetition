<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<!-- BANDEAU DE TITRE -->
			<div class="w3-top">
				<div class="w3-cell-row w3-light-grey">
					<button class="w3-cell w3-button w3-xlarge w3-cell-left" onclick="openElement('navigationPanel')">☰</button>
					<div class="w3-cell w3-cell-middle w3-center">
						<h3>Suivi compétition</h3>
					</div>
					<div class="w3-cell w3-cell-middle bandeau-titre">
						<img class="img img-bandeau-titre" src="../img/France-Judo-Rhone.png"/>
					</div>
				</div>
			</div>

			<!-- PANNEAU DE NAVIGATION -->
			<div class="w3-sidebar w3-bar-block w3-border-right w3-animate-left tas-navigation-panel" id="navigationPanel">
				<button onclick="closeElement('navigationPanel')" class="w3-bar-item w3-large">Fermer &times;</button>
				<xsl:if test="$affProchainCombats">
					<a class="w3-bar-item w3-button navButton" href="../common/se-prepare.html">Se prépare</a>
					<a class="w3-bar-item w3-button navButton w3-indigo" href="../common/prochains-combats.html">Prochains combats</a>
				</xsl:if>
				<xsl:if test="$affAffectationTapis">
					<a class="w3-bar-item w3-button navButton" href="../common/affectation_tapis.html">Affectations</a>
				</xsl:if>
				<a class="w3-bar-item w3-button navButton" href="../common/avancement.html">Avancements</a>
				<a class="w3-bar-item w3-button navButton" href="../common/classement.html">Classements</a>
			</div>

			<!-- Div vide pour aligner le contenu avec le bandeau de titre de taille fixe -->
			<div class="w3-container tas-filler-div">&nbsp;</div>

			<!-- CONTENU -->
			<!-- Boucle global sur les competitions en cours -->
			<xsl:for-each select="/competitions/competition">
				<xsl:if test="count(./epreuve) > 0">
					<!-- Nom de la competition -->
					<div class="w3-container w3-blue w3-center tas-competition-bandeau">
						<h4>
							<xsl:value-of select="./titre"/>
						</h4>
					</div>

					<div id="ProchainsCombats" class="w3-container w3-border pane w3-animate-left">
						<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
						<div class="w3-cell-row">
							<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
							<!-- Categorie F -->
							<xsl:if test="count(./epreuve[@sexe = 'F']) > 0">
								<div class="w3-panel w3-cell w3-mobile">
									<!-- La carte des donnees elle meme -->
									<div class="w3-card">
										<!-- entete avec un bouton permet d'ouvrir fermer le contenu de la carte -->
										<header class="w3-bar w3-light-green w3-large">
											<button class="w3-bar-item w3-light-green" onclick="toggleElement('ProchainsCombatsFemininesContentPanel')">
												<img class="img" id="ProchainsCombatsFemininesContentPanelCollapse" width="25" src="../img/up_circular-32.png" style="display: none;"/>
												<img class="img" id="ProchainsCombatsFemininesContentPanelExpand" width="25" src="../img/down_circular-32.png"/>
												Catégorie féminine
											</button>
										</header>
										<div class="w3-container" id="ProchainsCombatsFemininesContentPanel" style="display:none">
											<xsl:apply-templates select="./epreuve[@sexe = 'F']"/>
										</div>
									</div>
								</div>
							</xsl:if>
							<!-- Categorie M -->
							<xsl:if test="count(./epreuve[@sexe = 'M']) > 0">
								<div class="w3-panel w3-cell w3-mobile">
									<div class="w3-card">

										<header class="w3-bar w3-light-green w3-large">
											<button class="w3-bar-item w3-light-green" onclick="toggleElement('ProchainsCombatsMasculinsContentPanel')">
												<img class="img" id="ProchainsCombatsMasculinsContentPanelCollapse" width="25" src="../img/up_circular-32.png" style="display: none;"/>
												<img class="img" id="ProchainsCombatsMasculinsContentPanelExpand" width="25" src="../img/down_circular-32.png"/>
												Catégorie Masculine
											</button>
										</header>

										<div class="w3-container" id="ProchainsCombatsMasculinsContentPanel" style="display:none;">
											<xsl:apply-templates select="./epreuve[@sexe = 'M']"/>
										</div>
									</div>
								</div>
							</xsl:if>
							<!-- Sans Categorie -->
							<xsl:if test="count(./epreuve[not(@sexe)]) > 0">
								<div class="w3-panel w3-cell w3-mobile">
									<div class="w3-card">
										<header class="w3-bar w3-light-green w3-large">
											<button class="w3-bar-item w3-light-green"
												onclick="toggleElement('ProchainsCombatsNoSexeContentPanel')">
												<img class="img" id="ProchainsCombatsNoSexeContentPanelCollapse" width="25" src="../img/up_circular-32.png" style="display: none;"/>
												<img class="img" id="ProchainsCombatsNoSexeContentPanelExpand" width="25" src="../img/down_circular-32.png"/>
												Sans Catégorie
											</button>
										</header>

										<div class="w3-container" id="ProchainsCombatsNoSexeContentPanel" style="display:none;">
											<xsl:apply-templates select="./epreuve[not(@sexe)]"/>
										</div>
									</div>
								</div>
							</xsl:if>
						</div>
					</div>
				</xsl:if>
			</xsl:for-each>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- Bouton avancement par epreuve -->
	<xsl:template name="prochains_combats_epreuve" match="epreuve">
		<xsl:if test="count(./phases/phase[number(@typePhase) = 1]) > 0">
			<a class="w3-button w3-panel w3-card w3-block w3-pale-yellow w3-large w3-round-large w3-padding-small">
				<xsl:attribute name="href">
					<xsl:text>..</xsl:text>
					<xsl:value-of select="@directory"/>
					<xsl:text>/feuille_combats</xsl:text>
					<xsl:text>.html</xsl:text>
				</xsl:attribute>
				<xsl:value-of select="./@libelle"/>
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Poules</xsl:text>
			</a>
		</xsl:if>
		<xsl:if test="count(./phases/phase[number(@typePhase) = 2]) > 0">
			<a class="w3-button w3-panel w3-card w3-block w3-pale-yellow w3-large w3-round-large w3-padding-small">
				<xsl:attribute name="href">
					<xsl:text>..</xsl:text>
					<xsl:value-of select="@directory"/>
					<xsl:text>/feuille_combats</xsl:text>
					<xsl:text>.html</xsl:text>
				</xsl:attribute>
				<xsl:value-of select="./@libelle"/>
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Tableau</xsl:text>
			</a>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
