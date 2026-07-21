<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
	<!ENTITY nl "&#10;">
]>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/niveau_tour_combat.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="idgroupe"/>
	<xsl:param name="idcompetition"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>
	<xsl:param name="RefData"/>


	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
	<!-- valeur specifique du vainqueur en cas de Hikiwake -->
	<xsl:variable name="hikiwake" select="-2147483648"/>

	<xsl:variable name="couleur1" select="//competition[@ID = $idcompetition]/@couleur1"/>
	<xsl:variable name="couleur2" select="//competition[@ID = $idcompetition]/@couleur2"/>


	<xsl:variable name="selectedCompetition" select="/docroot/competitions/competition[@ID = $idcompetition]"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@EngagementsAbsents = 'true'" name="affEngagementsAbsents"/>
	<xsl:variable select="/docroot/SiteConfiguration/@EngagementsTousCombats = 'true'" name="affTousCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@EngagementsScoreGP = 'true'" name="affscoreGP"/>
	<xsl:variable select="/docroot/SiteConfiguration/@EngagementsPositionCombat = 'true'" name="affPositionCombat"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="/docroot/SiteConfiguration/@kinzas = 'Oui'" name="affKinzas"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>

	<xsl:variable select="$selectedCompetition/@type" name="typeCompetition"/>
	<xsl:variable select="$selectedCompetition/@niveau" name="niveauCompetition"/>

	<!-- En jujitsu, on affiche la discpline -->
	<xsl:variable select="$selectedCompetition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

	<!-- Le groupement selectionne -->
	<xsl:variable select="//groupeEngagements[@id = $idgroupe]" name="selectedGroupeEngagements"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
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
				<link type="text/css" rel="stylesheet">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'style-tableau.css')"/>
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
					<xsl:with-param name="affStatistiques" select="$affStatistiques"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="selectedItem" select="'engagements'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- CONTENU -->

				<!-- Nom de la competition + Groupe -->
				<div class="w3-container w3-blue w3-center tas-competition-bandeau">
					<div>
						<h4>
							<xsl:value-of select="$selectedCompetition/titre"/>
						</h4>
					</div>
					<div class="w3-card w3-indigo">
						<h5>
							<!-- Calcul le titre en fonction du type de groupement affEngagementsParEntite et du niveau de la competition -->
							<xsl:if test="$selectedGroupeEngagements/@sexe = 'F'">
								<xsl:text disable-output-escaping="yes">Féminines,</xsl:text>&nbsp;
							</xsl:if>
							<xsl:if test="$selectedGroupeEngagements/@sexe = 'M'">
								<xsl:text disable-output-escaping="yes">Masculins,</xsl:text>&nbsp;
							</xsl:if>

							<!-- Determine le nom du groupe a afficher -->
							<xsl:call-template name="LibelleGroupeStructure">
								<xsl:with-param name="typeGroupe" select="$selectedGroupeEngagements/@type"/>
								<xsl:with-param name="niveauCompetition" select="$niveauCompetition"/>
								<xsl:with-param name="entiteId" select="$selectedGroupeEngagements/@entite"/>
								<xsl:with-param name="RefData" select="$RefData"/>
								<xsl:with-param name="avecPrefixe" select="'true'" />
							</xsl:call-template>
						</h5>
					</div>
				</div>

				<!-- Calcul le regroupement des judokas-->
				<xsl:variable name="judokasGroupe" select="$selectedCompetition/judokas/judoka[
					@lib_sexe = $selectedGroupeEngagements/@sexe and (
						($selectedGroupeEngagements/@type = 1 and translate(substring(@nom,1,1), $lowercase, $uppercase) = translate($selectedGroupeEngagements/@entite, $lowercase, $uppercase)) or
						($selectedGroupeEngagements/@type = 2 and @club = $selectedGroupeEngagements/@entite) or
						($selectedGroupeEngagements/@type = 3 and @comite = $selectedGroupeEngagements/@entite) or
						($selectedGroupeEngagements/@type = 4 and @ligue = $selectedGroupeEngagements/@entite) or
						(($selectedGroupeEngagements/@type = 5 or $selectedGroupeEngagements/@type = 6) and @pays = $selectedGroupeEngagements/@entite)
					)
				]" />

				<!-- Extrait les absents et les presents -->
				<xsl:variable name="lesPresents" select="$judokasGroupe[@present = 'true']" />
				<xsl:variable name="lesAbsents" select="$judokasGroupe[not(@present = 'true')]" />

				<!-- Affiche les listes respectives -->
				<xsl:choose>
					<xsl:when test="count($lesPresents) > 0">
						<xsl:for-each select="$lesPresents">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<div class="w3-container w3-border w3-margin-top">
							<div class="w3-panel w3-pale-green w3-bottombar w3-border-green w3-border w3-center w3-large"> 
								Veuillez patienter la pesée des participants 
							</div>
						</div>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:if test="$affEngagementsAbsents">
					<xsl:call-template name="BlocAbsents">
						<xsl:with-param name="lesAbsents" select="$lesAbsents"/>
					</xsl:call-template>
				</xsl:if>
				
				<!-- Pied de page -->
				<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
					<script>
						<xsl:attribute name="src">
							<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
						</xsl:attribute>
					</script>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- TEMPLATES -->


	<!-- TEMPLATE UN JUDOKA -->
	<xsl:template name="UnJudoka" match="judoka">
		<xsl:param name="niveau"/>

		<xsl:variable name="apos">'</xsl:variable>

		<xsl:variable name="idJudoka" select="./@id"/>

		<!-- Bandeau repliable du judoka (ferme par defaut) -->
		<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
			<button class="w3-bar-item w3-light-blue w3-left-align">
				<xsl:attribute name="onclick">
					<xsl:value-of select="concat('togglePanel(',$apos,'judoka',@id,$apos,')')"/>
				</xsl:attribute>
				<img class="img" width="25" style="display: none;">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
					</xsl:attribute>
					<xsl:attribute name="id">
						<xsl:value-of select="concat('judoka',$idJudoka,'Collapse')"/>
					</xsl:attribute>
				</img>
				<img class="img" width="25">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
					</xsl:attribute>
					<xsl:attribute name="id">
						<xsl:value-of select="concat('judoka',$idJudoka,'Expand')"/>
					</xsl:attribute>
				</img>
				<div class="tas-judoka-identity">
					<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/><br/>

					<span class="w3-tiny w3-opacity w3-text-dark-grey">
						<xsl:variable name="clubNode" select="$RefData/structures/clubs/club[@ID = ./@club]"/>
						<xsl:variable name="comiteNode" select="$RefData/structures/comites/comite[@ID = $clubNode/@comite]"/>
						<xsl:variable name="ligueNode" select="$RefData/structures/ligues/ligue[@ID = $comiteNode/@ligue]"/>
						<xsl:variable name="paysNode" select="$RefData/structures/lesPays/pays[@ID = @pays]"/>

						<xsl:call-template name="LibelleStructure">
							<xsl:with-param name="ecartement" select="$niveauCompetition" />
							<xsl:with-param name="typeCompetition" select="$typeCompetition" />
							<xsl:with-param name="club" select="$clubNode/nomCourt" />
							<xsl:with-param name="comite" select="$comiteNode/@ID" />
							<xsl:with-param name="ligue" select="$ligueNode/nomCourt" />
							<xsl:with-param name="pays" select="$paysNode/@abr3" />
						</xsl:call-template>
					</span>
				</div>
			</button>
		</div>
		<!-- Le contenu du Judoka -->
		<div class="tasClosedPanelType tas-panel-tableau-combat" style="display: none;">
			<xsl:attribute name="id">
				<xsl:value-of select="concat('judoka',$idJudoka)"/>
			</xsl:attribute>
			<!-- La liste des combats dans lesquel le judo est présent -->
			<!-- Nb de combats pour ce judoka -->
			<xsl:variable name="nbCombatsJudoka">
				<xsl:choose>
					<xsl:when test="$affTousCombats">
						<xsl:value-of select="count($selectedCompetition/combats/combat[ (score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka)])"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="count($selectedCompetition/combats/combat[ (score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and (@vainqueur = 0 or @vainqueur = -1)  ])"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:choose>
				<xsl:when test="$nbCombatsJudoka > 0">
					<xsl:for-each select="$selectedCompetition/epreuves/epreuve">
						<xsl:variable name="idEpreuve" select="@ID"/>
						<!-- Nb de combats du judoka pour cette epreuve -->
						<xsl:variable name="nbCombatsJudokaEpreuve">
							<xsl:choose>
								<xsl:when test="$affTousCombats">
									<xsl:value-of select="count($selectedCompetition/combats/combat[ (score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve ])"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="count($selectedCompetition/combats/combat[ (score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve  and (@vainqueur = 0 or @vainqueur = -1)  ])"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:variable>

						<!-- On ne prend en compte que les epreuves pour lesquelles il y a des combats -->
						<xsl:if test="$nbCombatsJudokaEpreuve > 0">
							<div>
								<header class="w3-container w3-teal w3-large w3-padding-small">
									<xsl:choose>
										<xsl:when test="@sexe = 'F'">Féminines, </xsl:when>
										<xsl:when test="@sexe = 'M'">Masculins, </xsl:when>
									</xsl:choose>
									<xsl:value-of select="@nom"/>
									<xsl:if test="$affDiscipline">
										&nbsp;(<xsl:choose>
											<xsl:when test="./@discipline_competition = 2">Combat</xsl:when>
											<xsl:when test="./@discipline_competition = 3">NeWaza</xsl:when>
										</xsl:choose>
										- <xsl:value-of select="./@nom_cateage"/>)
									</xsl:if>
								</header>
								<div class="w3-container w3-cell w3-cell-middle w3-padding">
									<table class="tas-tableau-combat-participant">
										<tbody>
											<!-- Les combats n'ayant pas encore eu lieu sont tries par l'heure de programmation -->
											<xsl:for-each select="$selectedCompetition/combats/combat[(score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve and (@vainqueur = 0 or @vainqueur = -1)]">
												<xsl:sort select="@time_programmation" data-type="number" order="descending"/>
												<xsl:call-template name="UnCombat">
													<xsl:with-param name="niveau" select="$niveau"/>
												</xsl:call-template>
											</xsl:for-each>
											<xsl:if test="$affTousCombats">
												<!-- Les combats deja realises sont tries par la date de fin -->
												<xsl:for-each select="$selectedCompetition/combats/combat[(score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve and @vainqueur != 0 and @vainqueur != -1]">
													<xsl:sort select="@time_fin" data-type="number" order="descending"/>
													<xsl:call-template name="UnCombat">
														<xsl:with-param name="niveau" select="$niveau"/>
													</xsl:call-template>
												</xsl:for-each>
											</xsl:if>
										</tbody>
									</table>
								</div>
							</div>
						</xsl:if>
					</xsl:for-each>
				</xsl:when>
				<!-- Aucun combat pour ce Judoka -->
				<xsl:otherwise>
					<div class="w3-card w3-pale-yellow w3-center">
						<xsl:text disable-output-escaping="yes">Aucun combat assigné</xsl:text>
					</div>
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>

	<!-- TEMPLATE UN COMBAT -->
	<xsl:template name="UnCombat" match="combat">
		<xsl:param name="niveau"/>

		<xsl:variable name="epreuve" select="./@epreuve"/>
		<xsl:variable name="phase" select="./@phase"/>
		<xsl:variable name="typePhase" select="ancestor::competition/phases/phase[@id = $phase]/@typePhase"/>

		<xsl:variable name="judoka1" select="./score[1]/@judoka"/>
		<xsl:variable name="j1" select="ancestor::competition/judokas/judoka[@id = $judoka1]"/>
		<xsl:variable name="club1" select="$RefData/structures/clubs/club[@ID = $j1/@club]"/>
		<xsl:variable name="comite1" select="$RefData/structures/comites/comite[@ID = $club1/@comite]"/>
		<xsl:variable name="ligue1" select="$RefData/structures/ligues/ligue[@ID = $comite1/@ligue]"/>
		<xsl:variable name="pays1" select="$RefData/structures/lesPays/pays[@ID = $j1/@pays]"/>

		<xsl:variable name="judoka2" select="./score[2]/@judoka"/>
		<xsl:variable name="j2" select="ancestor::competition/judokas/judoka[@id = $judoka2]"/>
		<xsl:variable name="club2" select="$RefData/structures/clubs/club[@ID = $j2/@club]"/>
		<xsl:variable name="comite2" select="$RefData/structures/comites/comite[@ID = $club2/@comite]"/>
		<xsl:variable name="ligue2" select="$RefData/structures/ligues/ligue[@ID = $comite2/@ligue]"/>
		<xsl:variable name="pays2" select="$RefData/structures/lesPays/pays[@ID = $j2/@pays]"/>

		<!-- Si un des ID judoka vaut zero, c'est une place vide. Si judoka est null, c'est pas encore de combattant, on n'affiche rien -->
		<xsl:if test="count(./score[@judoka = 0]) = 0">
			<tr>
				<!-- Colonne pour l'affichage du Judoka 1 -->
				<td style="width:40%">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$judoka1 = 'null'">
								<xsl:text disable-output-escaping="yes">w3-sand w3-small w3-padding-small w3-center</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
								<xsl:choose>
									<xsl:when test="$couleur1 = 'Bleu'">
										<xsl:text disable-output-escaping="yes">w3-blue w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:when>
									<xsl:when test="$couleur1 = 'Rouge'">
										<xsl:text disable-output-escaping="yes">w3-red w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">w3-grey w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<!-- Affiche le nom du Judoka s'il n'est pas null, "En Attente" sinon -->
						<table class="w3-small">
						<tr>
							<!-- Le badge de vainqueur eventuellement -->
							<td class="tas-winner">
								<xsl:attribute name="rowspan">
									<xsl:choose>
										<xsl:when test="$judoka1 = 'null'">1</xsl:when>
										<xsl:otherwise>2</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:choose>
									<xsl:when test="$judoka1 != 'null' and @vainqueur != 0 and @vainqueur != -1 and $judoka1 = @vainqueur">
										<img class="img w3-circle w3-amber" width="20">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'winner-32.png')"/>
											</xsl:attribute>
										</img>
									</xsl:when>
									<xsl:when test="@vainqueur = $hikiwake">
										<img class="img w3-circle w3-white" width="15">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'equal-sign-32.png')"/>
											</xsl:attribute>
										</img>
									</xsl:when>
									<xsl:otherwise>
										&nbsp;
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<!-- Nom & Prénom du judoka -->
							<td>
								<xsl:choose>
									<xsl:when test="$judoka1 = 'null'">
										<!-- Combat en attente-->
										<img class="img" width="20">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
											</xsl:attribute>
										</img>
										<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="$j1/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$j1/@prenom"/>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
							<!-- L'entite du Judoka (si pas en attente) -->
							<xsl:if test="not($judoka1 = 'null')">
								<tr>
									<td class="w3-tiny">
										<xsl:call-template name="LibelleStructure">
											<xsl:with-param name="ecartement" select="$niveauCompetition" />
											<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
											<xsl:with-param name="club" select="$club1/nomCourt"  />
											<xsl:with-param name="comite" select="$comite1/@ID" />
											<xsl:with-param name="ligue" select="$ligue1/nomCourt"/>
											<xsl:with-param name="pays" select="$pays1/@abr3"/>
										</xsl:call-template>
									</td>
								</tr>
							</xsl:if>
					</table>
				</td>

				<!-- Colonne pour l'affichage des informations du combat -->
				<td class=" w3-pale-yellow w3-small w3-card w3-cell-middle w3-center"  style="width:20%">
					<table class="w3-tiny" style="width: 100%">
						<!--  Information sur le niveau du combat -->
						<tr>
							<td class="w3-center">
								<xsl:call-template name="NiveauTourCombat">
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="typePhase" select="$typePhase"/>
									<xsl:with-param name="repechage" select="./feuille/@repechage = 'true'"/>
								</xsl:call-template>
							</td>
						</tr>
						<!-- Le statut du combat (pas assigné, n° de tapis, resultat)-->
						<tr>
							<td class="w3-center">
								<xsl:choose>
									<!-- Combat pas encore termine, on affiche le tapis s'il est assigne -->
									<xsl:when test="./@vainqueur = 0 or ./@vainqueur = -1">

											<xsl:variable name="posCombat">
												<xsl:if test="$affPositionCombat">
													<xsl:call-template name="ordreCombatTapis">
													<xsl:with-param name="combat" select="."/>
												</xsl:call-template>
												</xsl:if>
											</xsl:variable>

											<xsl:choose>
											<xsl:when test ="./@tapis > 0">
												<xsl:choose>
													<xsl:when test="$affPositionCombat">
														<xsl:choose>
															<xsl:when test="$posCombat = 1">
																Tapis <xsl:value-of select="./@tapis"/> (<xsl:value-of select="$posCombat"/><sup>er</sup>)
															</xsl:when>
															<xsl:otherwise>
																Tapis <xsl:value-of select="./@tapis"/> (<xsl:value-of select="$posCombat"/><sup>ème</sup>)
															</xsl:otherwise>
														</xsl:choose>
													</xsl:when>
													<xsl:otherwise>
														Tapis <xsl:value-of select="./@tapis"/>		
													</xsl:otherwise>
												</xsl:choose>
											</xsl:when>
											<xsl:otherwise>
												A affecter
											</xsl:otherwise>
										</xsl:choose>
									</xsl:when>
									<xsl:otherwise>
										<!-- Combat termine, on va afficher le résultat du combat -->
										<xsl:choose>
											<xsl:when test="$affscoreGP">
												<xsl:call-template name="scoreCombatGagnantPerdant">
													<xsl:with-param name="combat" select="."/>
												</xsl:call-template>
											</xsl:when>
											<xsl:otherwise>
												<xsl:call-template name="scoreCombatPremierSecond">
													<xsl:with-param name="combat" select="."/>
												</xsl:call-template>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
					</table>
				</td>
				
				<!-- Colonne pour l'affichage du Judoka 2 -->
				<td style="width:40%">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$judoka2 = 'null'">
								<xsl:text disable-output-escaping="yes">w3-sand w3-small w3-padding-small w3-center</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
								<xsl:choose>
									<xsl:when test="$couleur2 = 'Bleu'">
										<xsl:text disable-output-escaping="yes">w3-blue w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:when>
									<xsl:when test="$couleur2 = 'Rouge'">
										<xsl:text disable-output-escaping="yes">w3-red w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">w3-grey w3-small w3-padding-small w3-right-align</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<!-- Affiche le nom du Judoka s'il n'est pas null, "En Attente" sinon -->
					<table class="w3-small">
						<tr>
							<!-- Nom & Prénom du judoka -->
							<td>
								<xsl:choose>
									<xsl:when test="$judoka2 = 'null'">
										<!-- Combat en attente-->
										<img class="img" width="20">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
											</xsl:attribute>
										</img>
										<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="$j2/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$j2/@prenom"/>
									</xsl:otherwise>
								</xsl:choose>
							</td>
							<!-- Le badge de vainqueur eventuellement -->
							<td class="tas-winner">
								<xsl:attribute name="rowspan">
									<xsl:choose>
										<xsl:when test="$judoka2 = 'null'">1</xsl:when>
										<xsl:otherwise>2</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:choose>
									<xsl:when test="$judoka2 != 'null' and @vainqueur != 0 and @vainqueur != -1 and $judoka2 = @vainqueur">
										<img class="img w3-circle w3-amber" width="20">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'winner-32.png')"/>
											</xsl:attribute>
										</img>
									</xsl:when>
									<xsl:when test="@vainqueur = $hikiwake">
										<img class="img w3-circle w3-white" width="15">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'equal-sign-32.png')"/>
											</xsl:attribute>
										</img>
									</xsl:when>
									<xsl:otherwise>
										&nbsp;
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
						<!-- L'entite du Judoka (si pas en attente) -->
						<xsl:if test="not($judoka2 = 'null')">
							<tr>
								<td class="w3-tiny">
									<xsl:call-template name="LibelleStructure">
										<xsl:with-param name="ecartement" select="$niveauCompetition" />
										<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
										<xsl:with-param name="club" select="$club2/nomCourt"  />
										<xsl:with-param name="comite" select="$comite2/@ID" />
										<xsl:with-param name="ligue" select="$ligue2/nomCourt"/>
										<xsl:with-param name="pays" select="$pays2/@abr3"/>
									</xsl:call-template>
								</td>
							</tr>
						</xsl:if>
					</table>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>

	<!-- TEMPLATE Score d'un combat G/P -->
	<xsl:template name="scoreCombatGagnantPerdant">
		<xsl:param name="combat"/>

		<xsl:variable name="kinzavainqueur">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@kinza"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/score[@judoka = $combat/@vainqueur]/@kinza"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="kinzaperdant">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@kinza"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/score[@judoka != $combat/@vainqueur]/@kinza"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="scorevainqueur">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@score"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@scorevainqueur"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="scoreperdant">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@score"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@scoreperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="penalitevainqueur">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@penalite"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@penvainqueur"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="penaliteperdant">
			<xsl:choose>
				<xsl:when test="./@vainqueur = $hikiwake">
					<!-- en cas de Hikiwake, on prend les resultats du 1er judoka, les 2 etant identiques -->
					<xsl:value-of select="$combat/score[1]/@penalite"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@penperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<div class="w3-center tas-resultat">
			<span class="w3-tiny">
				<xsl:choose>
					<xsl:when test="$scorevainqueur != ''">
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:value-of select="substring($scorevainqueur, 1, 3)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$scorevainqueur"/>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="$typeCompetition != '1'">
							<span class="w3-text-red">
								<xsl:choose>
									<xsl:when test="substring($penalitevainqueur, 1, 1) = '-' ">
										<xsl:value-of select="$penalitevainqueur"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="concat('-', $penalitevainqueur)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$affKinzas">
								<span class="w3-tiny w3-text-green">(<xsl:value-of select="$kinzavainqueur"/>)</span>
							</xsl:if>
						</xsl:if>
						<xsl:text disable-output-escaping="yes">/</xsl:text>
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:value-of select="substring($scoreperdant, 1, 3)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$scoreperdant"/>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="$typeCompetition != '1'">
							<span class="w3-text-red">
								<xsl:choose>
									<xsl:when test="substring($penaliteperdant, 1, 1) = '-' ">
										<xsl:value-of select="$penaliteperdant"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="concat('-', $penaliteperdant)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$affKinzas">
								<span class="w3-tiny w3-text-green">(<xsl:value-of select="$kinzaperdant"/>)</span>
							</xsl:if>
						</xsl:if>
					</xsl:when>
					<!-- Pas de score (combat pas encore realise) -->
					<xsl:otherwise>
						&nbsp;
					</xsl:otherwise>
				</xsl:choose>
			</span>
		</div>
	</xsl:template>

	<!-- TEMPLATE Score d'un combat 1er/2nd -->
	<xsl:template name="scoreCombatPremierSecond">
		<xsl:param name="combat"/>

		<xsl:variable name="kinzapremier">
			<xsl:value-of select="$combat/score[1]/@kinza"/>
		</xsl:variable>
		<xsl:variable name="kinzasecond">
			<xsl:value-of select="$combat/score[2]/@kinza"/>
		</xsl:variable>

		<xsl:variable name="scorepremier">
			<xsl:choose>
				<xsl:when test="$combat/@vainqueur = $hikiwake">
					<xsl:value-of select="$combat/score[1]/@score"/>
				</xsl:when>
				<xsl:when test="$combat/score[1]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/@scorevainqueur"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@scoreperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="scoresecond">
			<xsl:choose>
				<xsl:when test="$combat/@vainqueur = $hikiwake">
					<xsl:value-of select="$combat/score[2]/@score"/>
				</xsl:when>
				<xsl:when test="$combat/score[2]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/@scorevainqueur"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@scoreperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="penalitepremier">
			<xsl:choose>
				<xsl:when test="$combat/@vainqueur = $hikiwake">
					<xsl:value-of select="$combat/score[1]/@penalite"/>
				</xsl:when>
				<xsl:when test="$combat/score[1]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/@penvainqueur"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@penperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="penalitesecond">
			<xsl:choose>
				<xsl:when test="$combat/@vainqueur = $hikiwake">
					<xsl:value-of select="$combat/score[2]/@penalite"/>
				</xsl:when>
				<xsl:when test="$combat/score[2]/@judoka = $combat/@vainqueur">
					<xsl:value-of select="$combat/@penvainqueur"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$combat/@penperdant"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<div class="w3-center tas-resultat">
			<span class="w3-tiny">
				<xsl:choose>
					<xsl:when test="$scorepremier != ''">
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:value-of select="substring($scorepremier, 1, 3)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$scorepremier"/>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="$typeCompetition != '1'">
							<span class="w3-text-red">
								<xsl:choose>
									<xsl:when test="substring($penalitepremier, 1, 1) = '-' ">
										<xsl:value-of select="$penalitepremier"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="concat('-', $penalitepremier)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$affKinzas">
								<span class="w3-tiny w3-text-green">
									(<xsl:value-of select="$kinzapremier"/>)
								</span>
							</xsl:if>
						</xsl:if>
						<xsl:text disable-output-escaping="yes">/</xsl:text>
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:value-of select="substring($scoresecond, 1, 3)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$scoresecond"/>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="$typeCompetition != '1'">
							<span class="w3-text-red">
								<xsl:choose>
									<xsl:when test="substring($penalitesecond, 1, 1) = '-' ">
										<xsl:value-of select="$penalitesecond"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="concat('-', $penalitesecond)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$affKinzas">
								<span class="w3-tiny w3-text-green">
									(<xsl:value-of select="$kinzasecond"/>)
								</span>
							</xsl:if>
						</xsl:if>
					</xsl:when>
					<!-- Pas de score (combat pas encore realise) -->
					<xsl:otherwise>
						&nbsp;
					</xsl:otherwise>
				</xsl:choose>
			</span>
		</div>
	</xsl:template>

	<!-- TEMPLATE Score vide -->
	<xsl:template name="scoreVide">
		<div class="w3-left-align">
			<span class="w3-small">
				&nbsp;
			</span>
		</div>
	</xsl:template>

	<!-- TEMPLATE Calcul de la position du combat dans le deroulement du tapis -->
	<xsl:template name="ordreCombatTapis">
		<xsl:param name="combat"/>
		
		<!-- Selectionne tous les combats non termines sur le tapis du combat -->
		<xsl:for-each select="//combats/combat[@tapis = $combat/@tapis and (@vainqueur = 0 or @vainqueur = -1)]">
			<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>
			<xsl:if test="./@id = $combat/@id">
				<xsl:value-of select="position()"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<!-- TEMPLATE Absents -->
	<xsl:template name="BlocAbsents">
		<xsl:param name="lesAbsents"/>
		<xsl:variable name="apos">'</xsl:variable>

		<xsl:if test="count($lesAbsents) > 0">
			<div class="w3-container w3-large w3-bar w3-cell-middle tas-entete-section w3-taupe w3-margin-top">
				<button class="w3-bar-item w3-button" style="width: 100%; text-align: left; background-color: inherit;">
							<xsl:attribute name="onclick">
						<xsl:value-of select="concat('togglePanel(',$apos,'bloc-absents',$apos,')')"/>
					</xsl:attribute>

					<img class="img" width="25" style="display: none;">
						<xsl:attribute name="src">
							<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
						</xsl:attribute>
						<xsl:attribute name="id">bloc-absentsCollapse</xsl:attribute>
					</img>
					<img class="img" width="25">
						<xsl:attribute name="src">
							<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
						</xsl:attribute>
						<xsl:attribute name="id">bloc-absentsExpand</xsl:attribute>
					</img>

					Absents <span class="w3-medium" style="margin-left: 4px;">
						(<xsl:value-of select="count($lesAbsents)"/>)
					</span>
				</button>
			</div>

			<div id="bloc-absents" class="tasClosedPanelType" style="display: none;">

				<div class="w3-right-align tas-info-btn-container">
					<button id="info-bloc-absentsExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-absents')">
						<img width="18" alt="Info">
							<xsl:attribute name="src">
								<xsl:value-of select="concat($imgPath, 'info-32.png')"/>
							</xsl:attribute>
						</img>
					</button>
				</div>
				
				<div id="info-bloc-absents" class="tasClosedPanelType w3-panel w3-leftbar w3-border-taupe w3-paper w3-display-container tas-info-panel" style="display: none;">
					<span onclick="togglePanel('info-bloc-absents')" class="w3-button w3-display-topright w3-hover-taupe tas-info-close-btn">&times;</span>
					<div class="w3-tiny w3-text-dark-grey tas-info-text-container">
						<p class="tas-info-text">Ces judokas sont déclarés absents ou leur pesée n'a pas encore été validée.</p>
					</div>
				</div>

				<div class="w3-responsive w3-white w3-border w3-border-taupe w3-margin-bottom" style="margin: 0 8px;">
					<table class="w3-table w3-bordered w3-small w3-text-grey">
						<tbody>
							<xsl:for-each select="$lesAbsents">
								<xsl:sort select="@nom" order="ascending"/>
								<tr>
									<td class="w3-cell-middle">
										<strong>
											<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
										</strong>
									</td>
									<td class="w3-cell-middle w3-left-align">
										<span class="w3-tiny w3-opacity">
											<xsl:variable name="clubNode" select="$RefData/structures/clubs/club[@ID = current()/@club]"/>
											<xsl:variable name="comiteNode" select="$RefData/structures/comites/comite[@ID = $clubNode/@comite]"/>
											<xsl:variable name="ligueNode" select="$RefData/structures/ligues/ligue[@ID = $comiteNode/@ligue]"/>
											<xsl:variable name="paysNode" select="$RefData/structures/lesPays/pays[@ID = current()/@pays]"/>

											<xsl:call-template name="LibelleStructure">
												<xsl:with-param name="ecartement" select="$niveauCompetition" />
												<xsl:with-param name="typeCompetition" select="$typeCompetition" />
												<xsl:with-param name="club" select="$clubNode/nomCourt" />
												<xsl:with-param name="comite" select="$comiteNode/@ID" />
												<xsl:with-param name="ligue" select="$ligueNode/nomCourt" />
												<xsl:with-param name="pays" select="$paysNode/@abr3" />
											</xsl:call-template>
										</span>
									</td>
									<td class="w3-right-align w3-cell-middle">
										<span class="w3-tiny w3-opacity">
											<xsl:value-of select="@libepreuve"/>
										</span>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</div>
			</div>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>