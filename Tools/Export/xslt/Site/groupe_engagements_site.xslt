<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
	<!ENTITY nl "&#10;">
]>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>
	<xsl:import href="Tools/Export/xslt/Site/niveau_tour_combat.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="js"></xsl:param>
	<xsl:param name="idgroupe"></xsl:param>
	<xsl:param name="idcompetition"></xsl:param>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>


	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
	<!-- valeur specifique du vainqueur en cas de Hikiwake -->
	<xsl:variable name="hikiwake" select="-2147483648"/>
	
	<xsl:variable name="couleur1" select="//competition[@ID = $idcompetition]/@couleur1"/>
	<xsl:variable name="couleur2" select="//competition[@ID = $idcompetition]/@couleur2"/>


	<xsl:variable name="selectedCompetition" select="/competitions/competition[@ID = $idcompetition]"/>


	<xsl:variable select="$selectedCompetition/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="$selectedCompetition/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="$selectedCompetition/@EngagementsAbsents = 'true'" name="affEngagementsAbsents"/>
	<xsl:variable select="$selectedCompetition/@EngagementsTousCombats = 'true'" name="affTousCombats"/>
	<xsl:variable select="$selectedCompetition/@EngagementsScoreGP = 'true'" name="affscoreGP"/>
	<xsl:variable select="$selectedCompetition/@EngagementsPositionCombat = 'true'" name="affPositionCombat"/>
	<xsl:variable select="$selectedCompetition/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="$selectedCompetition/@kinzas = 'Oui'" name="affKinzas"/>
	<xsl:variable select="$selectedCompetition/@type" name="typeCompetition"/>
	<xsl:variable select="/competitions/competition[1]/@Logo" name="logo"/>

	<!-- En jujitsu, on affiche la discpline -->
	<xsl:variable select="$selectedCompetition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

		<!-- Le groupement selectionne -->
	<xsl:variable select="//groupeEngagements[@id = $idgroupe]" name="selectedGroupeEngagements"/>

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
			<link type="text/css" rel="stylesheet">
				<xsl:attribute name="href">
					<xsl:value-of select="concat($cssPath, 'style-tableau.css')"/>
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
				gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
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
				<xsl:with-param name="affActualiser" select="true()"/>
				<xsl:with-param name="selectedItem" select="'engagements'"/>
				<xsl:with-param name="pathToImg" select="$imgPath"/>
				<xsl:with-param name="pathToCommon" select="$commonPath"/>
			</xsl:call-template>

			<!-- CONTENU -->

			<!-- Nom de la competition + Groupe -->
			<div class="w3-container w3-blue w3-center tas-competition-bandeau">
				<div>
					<!-- TODO Verifier ici, le titre de la competition ne s'affiche pas -->
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
						<xsl:choose>
							<!-- Niveau Aucun (par Nom) 1 -->
							<xsl:when test="$selectedGroupeEngagements/@type = 1">
								<xsl:text disable-output-escaping="yes">Nom commençant par</xsl:text>&nbsp;<xsl:value-of select="$selectedGroupeEngagements/@entite"/>
							</xsl:when>
							<!-- Niveau Club 2 -->
							<xsl:when test="$selectedGroupeEngagements/@type = 2">
								<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;<xsl:value-of select="//club[@ID = $selectedGroupeEngagements/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau Departement 3 -->
							<xsl:when test="$selectedGroupeEngagements/@type = 3">
								<xsl:text disable-output-escaping="yes">Comité</xsl:text>&nbsp;<xsl:value-of select="//comite[@ID = $selectedGroupeEngagements/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau Ligue 3 -->
							<xsl:when test="$selectedGroupeEngagements/@type = 4">
								<xsl:text disable-output-escaping="yes">Ligue</xsl:text>&nbsp;<xsl:value-of select="//ligue[@ID = $selectedGroupeEngagements/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau National 5 -->
							<!-- Niveau International 6 -->
							<xsl:when test="$selectedGroupeEngagements/@type = 5 or $selectedGroupeEngagements/@type = 6">
								<xsl:value-of select="//pays[@ID = $selectedGroupeEngagements/@entite]/@nom"/>
							</xsl:when>
							<!-- Par defaut, on prend le club -->
							<xsl:otherwise>
								<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;<xsl:value-of select="//club[@ID = $selectedGroupeEngagements/@entite]/nom"/>
							</xsl:otherwise>
						</xsl:choose>
					</h5>
				</div>
			</div>

			<!-- Verifie la presence de judoka en fonction du groupement -->
			<xsl:variable name="nbJudoka">
				<xsl:choose>
					<!-- Niveau Aucun (par Nom) 1 -->
					<xsl:when test="$selectedGroupeEngagements/@type = 1">
						<xsl:choose>
							<xsl:when test="$affEngagementsAbsents">
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[translate(substring(@nom,1,1), $lowercase, $uppercase)  = translate($selectedGroupeEngagements/@entite, $lowercase, $uppercase) and @lib_sexe = $selectedGroupeEngagements/@sexe])"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[translate(substring(@nom,1,1), $lowercase, $uppercase)  = translate($selectedGroupeEngagements/@entite, $lowercase, $uppercase) and @lib_sexe = $selectedGroupeEngagements/@sexe and @present = 'true'])"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Niveau Club 2 -->
					<xsl:when test="$selectedGroupeEngagements/@type = 2">
						<xsl:choose>
							<xsl:when test="$affEngagementsAbsents">
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@club = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe])"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@club = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe and @present = 'true'])"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Niveau Departement 3 -->
					<xsl:when test="$selectedGroupeEngagements/@type = 3">
						<xsl:choose>
							<xsl:when test="$affEngagementsAbsents">
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@comite = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe])"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@comite = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe and @present = 'true'])"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Niveau Ligue 4 -->
					<xsl:when test="$selectedGroupeEngagements/@type = 4">
						<xsl:choose>
							<xsl:when test="$affEngagementsAbsents">
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@ligue = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe])"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@ligue = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe and @present = 'true'])"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Niveau National 5 -->
					<!-- Niveau International 6 -->
					<xsl:when test="$selectedGroupeEngagements/@type = 5 or $selectedGroupeEngagements/@type = 6">
						<xsl:choose>
							<xsl:when test="$affEngagementsAbsents">
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@pays = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe])"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="count($selectedCompetition/judokas/judoka[@pays = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe and @present = 'true'])"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Sinon, on ne sait pas comment selectionner les judokas ... -->
					<xsl:otherwise>0</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<!-- Selectionne les judokas en fonction du groupement -->
			<xsl:choose>
				<xsl:when test="$nbJudoka > 0">
					<!-- Niveau Aucun (par Nom) 1 -->
					<xsl:if test="$selectedGroupeEngagements/@type = 1">
						<xsl:for-each select="$selectedCompetition/judokas/judoka[translate(substring(@nom,1,1), $lowercase, $uppercase)  = translate($selectedGroupeEngagements/@entite, $lowercase, $uppercase) and @lib_sexe = $selectedGroupeEngagements/@sexe]">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
					<!-- Niveau Club 2 -->
					<xsl:if test="$selectedGroupeEngagements/@type = 2">
						<xsl:for-each select="$selectedCompetition/judokas/judoka[@club = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe]">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
					<!-- Niveau Departement 3 -->
					<xsl:if test="$selectedGroupeEngagements/@type = 3">
						<xsl:for-each select="$selectedCompetition/judokas/judoka[@comite = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe]">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
					<!-- Niveau Ligue 4 -->
					<xsl:if test="$selectedGroupeEngagements/@type = 4">
						<xsl:for-each select="$selectedCompetition/judokas/judoka[@ligue = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe]">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
					<!-- Niveau National 5 -->
					<!-- Niveau International 6 -->
					<xsl:if test="$selectedGroupeEngagements/@type = 5 or $selectedGroupeEngagements/@type = 6">
						<xsl:for-each select="$selectedCompetition/judokas/judoka[@pays = $selectedGroupeEngagements/@entite and @lib_sexe = $selectedGroupeEngagements/@sexe]">
							<xsl:sort select="@nom" order="ascending"/>
							<xsl:call-template name="UnJudoka">
								<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<!-- Aucun combat, on va afficher un message d'attente -->
					<div class="w3-container w3-border">
						<div class="w3-panel w3-pale-green w3-bottombar w3-border-green w3-border w3-center w3-large"> Veuillez patienter la pesée des participants </div>
					</div>
				</xsl:otherwise>
			</xsl:choose>
			<!-- Pied de page -->
			<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
					</xsl:attribute>
				</script>
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->


	<!-- TEMPLATE UN JUDOKA -->
	<xsl:template name="UnJudoka" match="judoka">
		<xsl:param name="niveau"/>

		<xsl:variable name="apos">'</xsl:variable>

		<xsl:variable name="idJudoka" select="./@id"/>

		<!-- ignore les judokas non present sauf si option d'affichage des absents -->
		<xsl:if test="@present = 'true' or $affEngagementsAbsents">
			<!-- Bandeau repliable du judoka (ferme par defaut) -->
			<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
				<button class="w3-bar-item w3-light-blue">
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
					<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
				</button>
			</div>
			<!-- Le contenu du Judoka -->
			<div class="tasClosedPanelType tas-panel-tableau-combat" style="display: none;">
				<xsl:attribute name="id">
					<xsl:value-of select="concat('judoka',$idJudoka)"/>
				</xsl:attribute>
				<xsl:choose>
					<xsl:when test="@present = 'true'">
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
					</xsl:when>
					<!-- Judoka absent -->
					<xsl:otherwise>
						<div class="w3-card w3-pale-red w3-center">
							<xsl:choose>
								<xsl:when test="@lib_sexe = 'F'">
									<xsl:text disable-output-escaping="yes">Judokate absente ou pas encore pesée</xsl:text>
								</xsl:when>
								<xsl:when test="@lib_sexe = 'M'">
									<xsl:text disable-output-escaping="yes">Judoka absent ou pas encore pesé</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text disable-output-escaping="yes">Judoka(te) absent(e) ou pas encore pesé(e)</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</xsl:if>
	</xsl:template>

	<!-- TEMPLATE UN COMBAT -->
	<xsl:template name="UnCombat" match="combat">
		<xsl:param name="niveau"></xsl:param>

		<xsl:variable name="epreuve" select="./@epreuve"/>
		<xsl:variable name="phase" select="./@phase"/>
		<xsl:variable name="typePhase" select="ancestor::competition/phases/phase[@id = $phase]/@typePhase"/>


		<xsl:variable name="judoka1" select="./score[1]/@judoka"/>
		<xsl:variable name="club1" select="ancestor::competition/judokas/judoka[@id = $judoka1]/@club"/>
		<xsl:variable name="comite1" select="ancestor::competition/judokas/judoka[@id = $judoka1]/@comite"/>
		<xsl:variable name="ligue1" select="ancestor::competition/judokas/judoka[@id = $judoka1]/@ligue"/>
		<xsl:variable name="pays1" select="ancestor::competition/judokas/judoka[@id = $judoka1]/@pays"/>

		<xsl:variable name="judoka2" select="./score[2]/@judoka"/>
		<xsl:variable name="club2" select="ancestor::competition/judokas/judoka[@id = $judoka2]/@club"/>
		<xsl:variable name="comite2" select="ancestor::competition/judokas/judoka[@id = $judoka2]/@comite"/>
		<xsl:variable name="ligue2" select="ancestor::competition/judokas/judoka[@id = $judoka2]/@ligue"/>
		<xsl:variable name="pays2" select="ancestor::competition/judokas/judoka[@id = $judoka2]/@pays"/>

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
										<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka1]/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka1]/@prenom"/>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
							<!-- L'entite du Judoka (si pas en attente) -->
							<xsl:if test="not($judoka1 = 'null')">
								<tr>
									<td class="w3-tiny">
										<xsl:choose>
											<!-- Niveau Club 2 -->
											<xsl:when test="$niveau = 2">
												<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
											</xsl:when>
											<!-- Niveau Departement 3 -->
											<xsl:when test="$niveau = 3">
												<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="//comite[@ID = $comite1]/nomCourt"/>
											</xsl:when>
											<!-- Niveau Ligue 4 -->
											<xsl:when test="$niveau = 4">
												<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
											</xsl:when>
											<!-- Niveau National 5 -->
											<!-- Niveau International 6 -->
											<xsl:when test="$niveau = 5 or $niveau = 6">
												<xsl:value-of select="//pays[@ID = $pays1]/@abr3"/>
											</xsl:when>
											<!-- Par defaut, on prend le club -->
											<xsl:otherwise>
												<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
											</xsl:otherwise>
										</xsl:choose>
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
										<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka2]/@nom"/>
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka2]/@prenom"/>
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
									<xsl:choose>
										<!-- Niveau Club 2 -->
										<xsl:when test="$niveau = 2">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Departement 3 -->
										<xsl:when test="$niveau = 3">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//comite[@ID = $comite2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Ligue 4 -->
										<xsl:when test="$niveau = 4">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau National 5 -->
										<!-- Niveau International 6 -->
										<xsl:when test="$niveau = 5 or $niveau = 6">
											<xsl:value-of select="//pays[@ID = $pays2]/@abr3"/>
										</xsl:when>
										<!-- Par defaut, on prend le club -->
										<xsl:otherwise>
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
										</xsl:otherwise>
									</xsl:choose>
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
			<xsl:value-of select="$combat/score[1]/@score"/>
		</xsl:variable>
		<xsl:variable name="scoresecond">
			<xsl:value-of select="$combat/score[2]/@score"/>
		</xsl:variable>
		<xsl:variable name="penalitepremier">
			<xsl:value-of select="$combat/score[1]/@penalite"/>
		</xsl:variable>
		<xsl:variable name="penalitesecond">
			<xsl:value-of select="$combat/score[2]/@penalite"/>
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

</xsl:stylesheet>