<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://example.com/2008/data">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>

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
	<xsl:variable select="/competitions/competition[1]/@Logo" name="logo"/>

	<xsl:template match="/*">
		<xsl:variable name="apos">'</xsl:variable>

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
				gUseAutoReload = false;
			</script>
			<title>
				Suivi Compétition - Engagements
			</title>
		</head>
		<body>
			<!-- ENTETE -->
			<xsl:call-template name="entete">
				<xsl:with-param name="logo" select="$logo"/>
				<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
				<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
				<xsl:with-param name="affEngagements" select="true()"/>
				<xsl:with-param name="affActualiser" select="false()"/>
				<xsl:with-param name="selectedItem" select="'engagements'"/>
				<xsl:with-param name="pathToImg" select="$imgPath"/>
				<xsl:with-param name="pathToCommon" select="$commonPath"/>
			</xsl:call-template>

			<!-- CONTENU -->
			<xsl:if test="count(/competitions/competition)=0 or count(//groupeEngagements)=0">
				<div class="w3-container w3-border">
					<div class="w3-panel w3-pale-green w3-bottombar w3-border-green w3-border w3-center w3-large"> Veuillez patienter le tirage des épreuves </div>
				</div>
			</xsl:if>

			<!-- Boucle global sur les competitions en cours -->
			<xsl:for-each select="/competitions/competition">
				<xsl:if test="count(./groupesEngagements/groupeEngagements) > 0">
					<xsl:call-template name="competition"/>
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
	<!-- Template pour le groupement -->
	<xsl:template name="competition" match="competition">

		<xsl:variable name="idcompetition" select="@ID"/>
		<xsl:variable name="apos">'</xsl:variable>
		<xsl:variable name="prefixPanel">
			<xsl:value-of select="concat('EngagementsComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>

		<!-- Nom de la competition -->
		<div class="w3-container w3-blue w3-center tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div class="w3-container w3-border pane w3-animate-left">
			<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
			<div class="w3-cell-row">
				<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
				<!-- Categorie F -->
				<xsl:if test="count(./groupesEngagements/groupeEngagements[@sexe = 'F']) > 0">
					<xsl:call-template name="UneCategorie">
						<xsl:with-param name="categorie" select="'F'"/>
					</xsl:call-template>
				</xsl:if>
				<!-- Categorie M -->
				<xsl:if test="count(./groupesEngagements/groupeEngagements[@sexe = 'M']) > 0">
					<xsl:call-template name="UneCategorie">
						<xsl:with-param name="categorie" select="'M'"/>
					</xsl:call-template>
				</xsl:if>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE CATEGORIE -->
	<xsl:template name="UneCategorie" match="competition">
		<xsl:param name="categorie"/>

		<xsl:variable name="idcompetition" select="@ID"/>
		<xsl:variable name="apos">'</xsl:variable>
		<xsl:variable name="prefixPanel">
			<xsl:value-of select="concat('EngagementsComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>
		<xsl:variable name="groupTab">
			<xsl:value-of select="concat('groupComp',$idcompetition, $categorie)"/>
		</xsl:variable>

		<div class="w3-panel w3-cell w3-mobile">
			<div class="w3-card">
				<!-- Le bandeau de la categorie -->
				<header class="w3-bar w3-light-green w3-large">
					<button class="w3-bar-item w3-light-green">
						<xsl:attribute name="onclick">
							<xsl:value-of select="concat('togglePanel(',$apos,$prefixPanel, $categorie, $apos,')')"/>
						</xsl:attribute>
						<img class="img" width="25" style="display: none;">
							<xsl:attribute name="src">
								<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
							</xsl:attribute>
							<xsl:attribute name="id">
								<xsl:value-of select="concat($prefixPanel, $categorie, 'Collapse')"/>
							</xsl:attribute>
						</img>
						<img class="img" width="25">
							<xsl:attribute name="src">
								<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
							</xsl:attribute>
							<xsl:attribute name="id">
								<xsl:value-of select="concat($prefixPanel, $categorie, 'Expand')"/>
							</xsl:attribute>
						</img>
						<xsl:choose>
							<xsl:when test="$categorie = 'F'">
								Féminines
							</xsl:when>
							<xsl:when test="$categorie = 'M'">
								Masculins
							</xsl:when>
							<xsl:otherwise>
								Sans catégorie
							</xsl:otherwise>
						</xsl:choose>
					</button>
				</header>
				
				<div class="tasClosedPanelType w3-row w3-container" style="display:none;">
					<xsl:attribute name="id">
						<xsl:value-of select="concat($prefixPanel, $categorie)"/>
					</xsl:attribute>

					<!-- quel est le plus haut niveau de la liste -->
					<xsl:variable name="maxNiveau">
						<xsl:for-each select="//competition[@ID = $idcompetition]/groupesEngagements">
							<xsl:sort select="@type" data-type="number" order="ascending"/>
							<xsl:if test="position()=last()">
								<last>
									<xsl:value-of select="@type"/>
								</last>
							</xsl:if>
						</xsl:for-each>
					</xsl:variable>

					<!-- Le bandeau de selection d'un groupement -->
					<div class="w3-bar w3-light-gray">
						<xsl:for-each select="//competition[@ID = $idcompetition]/groupesEngagements">
							<xsl:sort order="descending" select="@type"/>
							<!-- Niveau Aucun (par Nom) 1 -->
							<xsl:if test="@type = 1">
								<button>
									<xsl:attribute name="id">btnNom</xsl:attribute>
									<xsl:attribute name="data-tabgroup">
										<xsl:value-of select="$groupTab"/>
									</xsl:attribute>
									<xsl:attribute name="class">tasTabBtnType w3-bar-item w3-button w3-round w3-margin-left
										<xsl:if test="@type = $maxNiveau">
											<xsl:text> w3-indigo</xsl:text>
										</xsl:if>
									</xsl:attribute>
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('openTab(',$apos,$groupTab,$apos,', ', $apos, 'Nom',$apos,', true)')"/>
									</xsl:attribute>
									Nom
								</button>
							</xsl:if>

							<!-- Niveau Club 2 -->
							<xsl:if test="@type = 2">
								<button>
									<xsl:attribute name="id">btnClub</xsl:attribute>
									<xsl:attribute name="data-tabgroup">
										<xsl:value-of select="$groupTab"/>
									</xsl:attribute>
									<xsl:attribute name="class">tasTabBtnType w3-bar-item w3-button w3-round w3-margin-left
										<xsl:if test="@type = $maxNiveau">
											<xsl:text> w3-indigo</xsl:text>
										</xsl:if>
									</xsl:attribute>
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('openTab(',$apos,$groupTab,$apos,', ', $apos, 'Club',$apos,', true)')"/>
									</xsl:attribute>
									Club
								</button>
							</xsl:if>

							<!-- Niveau Departement 3 -->
							<xsl:if test="@type = 3">
								<button>
									<xsl:attribute name="id">btnComite</xsl:attribute>
									<xsl:attribute name="data-tabgroup">
										<xsl:value-of select="$groupTab"/>
									</xsl:attribute>
									<xsl:attribute name="class">tasTabBtnType w3-bar-item w3-button w3-round w3-margin-left
										<xsl:if test="@type = $maxNiveau">
											<xsl:text> w3-indigo</xsl:text>
										</xsl:if>
									</xsl:attribute>
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('openTab(',$apos,$groupTab,$apos,', ', $apos, 'Comite',$apos,', true)')"/>
									</xsl:attribute>
									Comité
								</button>
							</xsl:if>

							<!-- Niveau Ligue 4 -->
							<xsl:if test="@type = 4">
								<button>
									<xsl:attribute name="id">btnLigue</xsl:attribute>
									<xsl:attribute name="data-tabgroup">
										<xsl:value-of select="$groupTab"/>
									</xsl:attribute>
									<xsl:attribute name="class">tasTabBtnType w3-bar-item w3-button w3-round w3-margin-left
										<xsl:if test="@type = $maxNiveau">
											<xsl:text> w3-indigo</xsl:text>
										</xsl:if>
									</xsl:attribute>
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('openTab(',$apos,$groupTab,$apos,', ', $apos, 'Ligue',$apos,', true)')"/>
									</xsl:attribute>
									Ligue
								</button>
							</xsl:if>

							<!-- Niveau National 5 -->
							<!-- Niveau International 6 -->
							<xsl:if test="@type = 5 or @type = 6">
								<button>
									<xsl:attribute name="id">btnPays</xsl:attribute>
									<xsl:attribute name="data-tabgroup">
										<xsl:value-of select="$groupTab"/>
									</xsl:attribute>
									<xsl:attribute name="class">tasTabBtnType w3-bar-item w3-button w3-round w3-margin-left
										<xsl:if test="@type = $maxNiveau">
											<xsl:text> w3-indigo</xsl:text>
										</xsl:if>
									</xsl:attribute>
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('openTab(',$apos,$groupTab,$apos,', ', $apos, 'Pays',$apos,', true)')"/>
									</xsl:attribute>
									Pays
								</button>
							</xsl:if>
						</xsl:for-each>
					</div>

					<!-- Le contenu des onglets -->
					<xsl:for-each select="//competition[@ID = $idcompetition]/groupesEngagements">
						<xsl:sort order="descending" select="@type"/>


						<div class="w3-container tasTabType">
							<xsl:attribute name="data-tabgroup">
								<xsl:value-of select="$groupTab"/>
							</xsl:attribute>

							<!-- Niveau Aucun (par Nom) 1 -->
							<xsl:if test="@type = 1">
								<xsl:attribute name="id">Nom</xsl:attribute>
								<xsl:attribute name="style">
									<xsl:choose>
										<xsl:when test="@type = $maxNiveau">
											display: block;
										</xsl:when>
										<xsl:otherwise>display: none;</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:apply-templates select="./groupeEngagements[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="@entite"/>
								</xsl:apply-templates>
							</xsl:if>

							<!-- Niveau Club 2 -->
							<xsl:if test="@type = 2">
								<xsl:attribute name="id">Club</xsl:attribute>
								<xsl:attribute name="style">
									<xsl:choose>
										<xsl:when test="@type = $maxNiveau">
											display: block;
										</xsl:when>
										<xsl:otherwise>display: none;</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:apply-templates select="./groupeEngagements[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="//club[@ID = current()/@entite]/nom"/>
								</xsl:apply-templates>
							</xsl:if>

							<!-- Niveau Departement 3 -->
							<xsl:if test="@type = 3">
								<xsl:attribute name="id">Comite</xsl:attribute>
								<xsl:attribute name="style">
									<xsl:choose>
										<xsl:when test="@type = $maxNiveau">
											display: block;
										</xsl:when>
										<xsl:otherwise>display: none;</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:apply-templates select="./groupeEngagements[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="//comite[@ID = current()/@entite]/nom"/>
								</xsl:apply-templates>
							</xsl:if>

							<!-- Niveau Ligue 4 -->
							<xsl:if test="@type = 4">
								<xsl:attribute name="id">Ligue</xsl:attribute>
								<xsl:attribute name="style">
									<xsl:choose>
										<xsl:when test="@type = $maxNiveau">
											display: block;
										</xsl:when>
										<xsl:otherwise>display: none;</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:apply-templates select="./groupeEngagements[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="//ligue[@ID = current()/@entite]/nom"/>
								</xsl:apply-templates>
							</xsl:if>

							<!-- Niveau National 5 -->
							<!-- Niveau International 6 -->
							<xsl:if test="@type = 5 or @type = 6">
								<xsl:attribute name="id">Pays</xsl:attribute>
								<xsl:attribute name="style">
									<xsl:choose>
										<xsl:when test="@type = $maxNiveau">
											display: block;
										</xsl:when>
										<xsl:otherwise>display: none;</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:apply-templates select="./groupeEngagements[@competition = $idcompetition and @sexe = $categorie]">
									<xsl:sort order="ascending" select="//pays[@ID = current()/@entite]/@nom"/>
								</xsl:apply-templates>
							</xsl:if>
						</div>
					</xsl:for-each>
				</div>
			</div>
		</div>
	</xsl:template>
	
	<!-- TEMPLATE Bouton groupement -->
	<xsl:template name="groupement" match="groupeEngagements">
		<!-- Determine le nom a afficher selon le niveau de la competition -->
		<xsl:variable name="entiteId">
			<xsl:value-of select="./@entite"/>
		</xsl:variable>
		<xsl:variable name="entiteNom">
			<xsl:choose>
				<!-- Niveau Aucun (par Nom) 1 -->
				<xsl:when test="./@type = 1">
					<xsl:value-of select="$entiteId"/>
				</xsl:when>
				<!-- Niveau Club 2 -->
				<xsl:when test="./@type = 2">
					<xsl:value-of select="//club[@ID = $entiteId]/nom"/>
				</xsl:when>
				<!-- Niveau Departement 3 -->
				<xsl:when test="./@type = 3">
					<xsl:value-of select="//comite[@ID = $entiteId]/nom"/>
				</xsl:when>
				<!-- Niveau Ligue 3 -->
				<xsl:when test="./@type = 4">
					<xsl:value-of select="//ligue[@ID = $entiteId]/nom"/>
				</xsl:when>
				<!-- Niveau National 5 -->
				<!-- Niveau International 6 -->
				<xsl:when test="./@type = 5 or ./@type = 6">
					<xsl:value-of select="//pays[@ID = $entiteId]/@nom"/>
				</xsl:when>
				<!-- Par defaut, on prend le club -->
				<xsl:otherwise>
					<xsl:value-of select="//club[@ID = $entiteId]/nom"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<div>
			<xsl:attribute name="class">
				<xsl:choose>
					<!-- Affiche les noms par Lettre avec un bouton circulaire -->
					<xsl:when test="./@type = '1'">w3-col s3 m2 l1 w3-center w3-padding</xsl:when>
					<xsl:otherwise></xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<a>
				<xsl:attribute name="class">
					<xsl:choose>
						<!-- Affiche les noms par Lettre avec un bouton circulaire -->
						<xsl:when test="./@type = '1'">w3-button w3-card w3-circle w3-xlarge w3-pale-yellow </xsl:when>
						<xsl:otherwise>w3-button w3-panel w3-card w3-block w3-pale-yellow w3-small w3-round-large w3-padding-small</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="href">
					<xsl:value-of select="concat($competitionPath, 'engagements/', @id, '/groupe_engagements.html')"/>
				</xsl:attribute>
				<!-- Utilise le nom de l'entite retenue en fonction du niveau -->
				<xsl:value-of select="$entiteNom"/>
			</a>
		</div>
	</xsl:template>
</xsl:stylesheet>