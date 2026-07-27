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
	<xsl:param name="RefData"/>
	<xsl:param name="SiteRoutes"/>

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
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:variable select="$selectedCompetition/@type" name="typeCompetition"/>
	<xsl:variable select="$selectedCompetition/@niveau" name="niveauCompetition"/>

	<!-- En jujitsu, on affiche la discipline -->
	<xsl:variable select="$selectedCompetition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

	<!-- Le groupement selectionne -->
	<xsl:variable select="//groupeEngagements[@id = $idgroupe]" name="selectedGroupeEngagements"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<META http-equiv="Content-Type" content="text/html; charset=utf-8"/>
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'w3.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-common.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-tableau.css')}"/>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
					gDefaultAutoReload = <xsl:value-of select="$actualisationClientDefaut"/>;
				</script>

				<script src="{concat($jsPath, 'site-display.js')}"/>

				<title>Suivi Compétition - Engagements</title>
			</head>
			<body>
				<!-- ENTETE -->
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
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

				<!-- Récupération de l'URL des statistiques pour CE groupe -->
				<xsl:variable name="urlStatistiques" select="$SiteRoutes//routeGroupe[@groupe = $idgroupe and @typeGroupe = 'statistique']/@urlGroupe" />
				
				<!-- Nom de la competition + Groupe (Modernisé) -->
				<!-- Nom de la competition + Groupe (Modernisé avec Flexbox) -->
				<div class="tas-competition-bandeau">
					<!-- Bloc de gauche : Titres -->
					<div>
						<h4>
							<xsl:value-of select="$selectedCompetition/titre"/>
						</h4>
						<!-- Conteneur pour cibler le Flexbox dans ton CSS externe -->
						<h5 class="tas-groupe-titre-container">
							<span>
								<xsl:if test="$selectedGroupeEngagements/@sexe = 'F'">Féminines,&nbsp;</xsl:if>
								<xsl:if test="$selectedGroupeEngagements/@sexe = 'M'">Masculins,&nbsp;</xsl:if>

								<!-- Determine le nom du groupe a afficher -->
								<xsl:call-template name="LibelleGroupeStructure">
									<xsl:with-param name="typeGroupe" select="$selectedGroupeEngagements/@type"/>
									<xsl:with-param name="niveauCompetition" select="$niveauCompetition"/>
									<xsl:with-param name="entiteId" select="$selectedGroupeEngagements/@entite"/>
									<xsl:with-param name="RefData" select="$RefData"/>
									<xsl:with-param name="avecPrefixe" select="'true'" />
								</xsl:call-template>
							</span>

							<!-- Bouton Statistiques propre -->
							<xsl:if test="$urlStatistiques != ''">
								<a href="{$urlStatistiques}" class="w3-button w3-circle tas-icon-btn tas-btn-statistiques" title="Voir les statistiques de ce groupe">
									<!-- Ajustez le nom de l'image selon votre nomenclature -->
									<img class="tas-theme-icon" src="{$imgPath}statistics-32.png" width="20" />
								</a>
							</xsl:if>
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

				<div class="w3-padding-small">
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
							<!-- Le message de "vide" ne s'affiche QUE si l'option des absents est fausse -->
							<xsl:if test="not($affEngagementsAbsents)">
								<div class="ios-card tas-empty-state">
									Aucun judoka pour ce groupe. Vérifiez si les pesées correspondantes ont été effectuées.
								</div>
							</xsl:if>
						</xsl:otherwise>					</xsl:choose>

					<xsl:if test="$affEngagementsAbsents">
						<xsl:call-template name="BlocAbsents">
							<xsl:with-param name="lesAbsents" select="$lesAbsents"/>
						</xsl:call-template>
					</xsl:if>
				</div>

				<!-- Pied de page -->
				<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
					<script src="{concat($jsPath, 'footer_script.js')}"/>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- ===================================================================== -->
	<!-- TEMPLATES -->
	<!-- ===================================================================== -->

	<!-- TEMPLATE UN JUDOKA -->
	<xsl:template name="UnJudoka" match="judoka">
		<xsl:param name="niveau"/>
		<xsl:variable name="idJudoka" select="./@id"/>

		<!-- Bandeau repliable du judoka (Bouton Accordéon iOS) -->
		<div>
			<button class="ios-accordion-btn" onclick="togglePanel('judoka{$idJudoka}')">
				<div class="tas-judoka-identity">
					<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/><br/>
					<span class="w3-tiny text-muted" style="font-weight: 400;">
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
				<div>
					<img class="tas-accordion-icon tas-icon-hidden" id="judoka{$idJudoka}Collapse" src="{$imgPath}up_circular-32.png"/>
					<img class="tas-accordion-icon tas-icon-visible" id="judoka{$idJudoka}Expand" src="{$imgPath}down_circular-32.png"/>
				</div>
			</button>

			<!-- Le contenu du Judoka -->
			<div class="tasClosedPanelType tas-accordion-content-hidden tas-panel-tableau-combat" id="judoka{$idJudoka}">
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

							<!-- Récupération de l'URL de la phase active pour CETTE épreuve -->
							<xsl:variable name="urlAvancement" select="$SiteRoutes//routeEpreuve[@epreuve = $idEpreuve]/@urlAvancement" />
							
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

							<xsl:if test="$nbCombatsJudokaEpreuve > 0">
								<div class="ios-card w3-margin-bottom">
									<div class="tas-card-header w3-small" style="display: flex; justify-content: space-between; align-items: center;">

										<!-- Titre de l'épreuve (à gauche) -->
										<div>
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
										</div>

										<!-- Bouton d'accès à l'avancement (à droite) -->
										<xsl:if test="$urlAvancement != ''">
											<a href="{$urlAvancement}" class="w3-button w3-circle tas-icon-btn" style="padding: 4px;" title="Voir l'avancement de l'épreuve">
												<img class="tas-theme-icon" src="{$imgPath}tree_structure-32.png" width="16" style="vertical-align: middle;"/>
											</a>
										</xsl:if>

									</div>
									<div class="w3-padding-small">
										<table class="tas-tableau-combat-participant">
											<tbody>
												<xsl:for-each select="$selectedCompetition/combats/combat[(score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve and (@vainqueur = 0 or @vainqueur = -1)]">
													<xsl:sort select="@time_programmation" data-type="number" order="descending"/>
													<xsl:call-template name="UnCombat">
														<xsl:with-param name="niveau" select="$niveau"/>
													</xsl:call-template>
												</xsl:for-each>
												<xsl:if test="$affTousCombats">
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
					<!-- Aucun combat pour ce Judoka (Empty State iOS) -->
					<xsl:otherwise>
						<div class="ios-card tas-empty-state">
							Aucun combat assigné
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE UN COMBAT (Modernisé comme feuille_matchs_site.xslt) -->
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

		<xsl:if test="count(./score[@judoka = 0]) = 0">
			<tr>
				<!-- Judoka 1 (Gauche) -->
				<td style="width:40%; padding-right:4px;">
					<xsl:choose>
						<xsl:when test="$judoka1 = 'null'">
							<div class="tas-upcoming-card-waiting">
								<img class="img" width="25" src="{$imgPath}sablier.png"/>
								<span>En Attente</span>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<div>
								<xsl:attribute name="class">
									<xsl:text>tas-upcoming-card </xsl:text>
									<xsl:choose>
										<xsl:when test="$couleur1 = 'Bleu'">belt-right-blue</xsl:when>
										<xsl:when test="$couleur1 = 'Rouge'">belt-right-red</xsl:when>
										<xsl:otherwise>belt-right-neutral</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<!-- L'entête contient la médaille du vainqueur (à gauche) et le nom (à droite) -->
								<header class="w3-small" style="font-weight: 600; display: flex; align-items: center; justify-content: flex-end; gap: 8px;">
									<xsl:choose>
										<xsl:when test="$judoka1 != 'null' and @vainqueur != 0 and @vainqueur != -1 and $judoka1 = @vainqueur">
											<img width="18" src="{$imgPath}winner-32.png" class="tas-theme-icon"/>
										</xsl:when>
										<xsl:when test="@vainqueur = $hikiwake">
											<img width="14" src="{$imgPath}equal-sign-32.png" class="tas-theme-icon"/>
										</xsl:when>
									</xsl:choose>
									<span>
										<xsl:value-of select="$j1/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$j1/@prenom"/>
									</span>
								</header>
								<footer class="w3-tiny text-muted">
									<xsl:call-template name="LibelleStructure">
										<xsl:with-param name="ecartement" select="$niveauCompetition" />
										<xsl:with-param name="typeCompetition" select="$typeCompetition" />
										<xsl:with-param name="club" select="$club1/nomCourt" />
										<xsl:with-param name="comite" select="$comite1/@ID" />
										<xsl:with-param name="ligue" select="$ligue1/nomCourt" />
										<xsl:with-param name="pays" select="$pays1/@abr3" />
									</xsl:call-template>
								</footer>
							</div>
						</xsl:otherwise>
					</xsl:choose>
				</td>

				<!-- Info Combat Centrale -->
				<td class="w3-cell-middle w3-center" style="width:20%">
					<div class="tas-upcoming-info">
						<header class="w3-small" style="font-weight: 600;">
							<xsl:call-template name="NiveauTourCombat">
								<xsl:with-param name="combat" select="."/>
								<xsl:with-param name="typePhase" select="$typePhase"/>
								<xsl:with-param name="repechage" select="./feuille/@repechage = 'true'"/>
							</xsl:call-template>
						</header>
						<footer class="w3-tiny" style="margin-top: 4px;">
							<xsl:choose>
								<xsl:when test="./@vainqueur = 0 or ./@vainqueur = -1">
									<xsl:variable name="posCombat">
										<xsl:if test="$affPositionCombat">
											<xsl:call-template name="ordreCombatTapis">
												<xsl:with-param name="combat" select="."/>
											</xsl:call-template>
										</xsl:if>
									</xsl:variable>
									<xsl:choose>
										<xsl:when test="./@tapis > 0">
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
										<xsl:otherwise>À affecter</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<!-- Affichage du Score si combat terminé -->
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
						</footer>
					</div>
				</td>

				<!-- Judoka 2 (Droite) -->
				<td style="width:40%; padding-left:4px;">
					<xsl:choose>
						<xsl:when test="$judoka2 = 'null'">
							<div class="tas-upcoming-card-waiting">
								<img class="img" width="25" src="{$imgPath}sablier.png"/>
								<span>En Attente</span>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<div>
								<xsl:attribute name="class">
									<xsl:text>tas-upcoming-card </xsl:text>
									<xsl:choose>
										<xsl:when test="$couleur2 = 'Bleu'">belt-left-blue</xsl:when>
										<xsl:when test="$couleur2 = 'Rouge'">belt-left-red</xsl:when>
										<xsl:otherwise>belt-left-neutral</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<!-- L'entête contient le nom (à gauche) et la médaille du vainqueur (à droite) -->
								<header class="w3-small" style="font-weight: 600; display: flex; align-items: center; justify-content: flex-start; gap: 8px;">
									<span>
										<xsl:value-of select="$j2/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$j2/@prenom"/>
									</span>
									<xsl:choose>
										<xsl:when test="$judoka2 != 'null' and @vainqueur != 0 and @vainqueur != -1 and $judoka2 = @vainqueur">
											<img width="18" src="{$imgPath}winner-32.png" class="tas-theme-icon"/>
										</xsl:when>
										<xsl:when test="@vainqueur = $hikiwake">
											<img width="14" src="{$imgPath}equal-sign-32.png" class="tas-theme-icon"/>
										</xsl:when>
									</xsl:choose>
								</header>
								<footer class="w3-tiny text-muted">
									<xsl:call-template name="LibelleStructure">
										<xsl:with-param name="ecartement" select="$niveauCompetition" />
										<xsl:with-param name="typeCompetition" select="$typeCompetition" />
										<xsl:with-param name="club" select="$club2/nomCourt" />
										<xsl:with-param name="comite" select="$comite2/@ID" />
										<xsl:with-param name="ligue" select="$ligue2/nomCourt" />
										<xsl:with-param name="pays" select="$pays2/@abr3" />
									</xsl:call-template>
								</footer>
							</div>
						</xsl:otherwise>
					</xsl:choose>
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
							<span class="fj-red">
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
								<span class="w3-tiny w3-text-green">
									(<xsl:value-of select="$kinzavainqueur"/>)
								</span>
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
							<span class="fj-red">
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
								<span class="w3-tiny w3-text-green">
									(<xsl:value-of select="$kinzaperdant"/>)
								</span>
							</xsl:if>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>&nbsp;</xsl:otherwise>
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
							<span class="fj-red">
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
							<span class="fj-red">
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
					<xsl:otherwise>&nbsp;</xsl:otherwise>
				</xsl:choose>
			</span>
		</div>
	</xsl:template>

	<!-- TEMPLATE Calcul de la position du combat dans le deroulement du tapis -->
	<xsl:template name="ordreCombatTapis">
		<xsl:param name="combat"/>
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

		<xsl:if test="count($lesAbsents) > 0">
			<!-- Accordéon principal pour les Absents -->
			<div class="w3-margin-top">
				<button class="ios-accordion-btn ios-accordion-btn-absent" onclick="togglePanel('bloc-absents')">
					<span>
						Absents <span class="w3-medium" style="margin-left: 4px;">
							(<xsl:value-of select="count($lesAbsents)"/>)
						</span>
					</span>
					<div>
						<img class="tas-accordion-icon tas-icon-hidden" id="bloc-absentsCollapse" src="{$imgPath}up_circular-32.png"/>
						<img class="tas-accordion-icon tas-icon-visible" id="bloc-absentsExpand" src="{$imgPath}down_circular-32.png"/>
					</div>
				</button>
			</div>

			<div id="bloc-absents" class="tasClosedPanelType tas-accordion-content-hidden">

				<!-- Callout d'Information modernisé -->
				<div id="info-bloc-absents" class="tas-callout">
					<button onclick="this.parentElement.style.display='none'" class="tas-callout-close">&times;</button>
					<div>Ces judokas sont déclarés absents ou leur pesée n'a pas encore été validée.</div>
				</div>

				<!-- Table des Absents dans une carte iOS -->
				<div class="ios-card w3-margin-bottom">
					<table class="w3-table w3-bordered w3-small text-muted">
						<tbody>
							<xsl:for-each select="$lesAbsents">
								<xsl:sort select="@nom" order="ascending"/>
								<tr>
									<td class="w3-cell-middle">
										<strong style="color: var(--text-color);">
											<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
										</strong>
									</td>
									<td class="w3-cell-middle w3-left-align">
										<span class="w3-tiny">
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
										<span class="w3-tiny">
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