<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
	<!ENTITY nl "&#10;">
]>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>

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
	<xsl:param name="RefData"/>

	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

	<xsl:variable name="selectedCompetition" select="/docroot/competitions/competition[@ID = $idcompetition]"/>
	<xsl:variable select="$selectedCompetition/@type" name="typeCompetition"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>

	<xsl:variable select="//groupeStatistiques[@id = $idgroupe]" name="selectedGroupe"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

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
						<xsl:value-of select="concat($cssPath, 'style-statistiques.css')"/>
					</xsl:attribute>
				</link>

				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-display.js')"/>
					</xsl:attribute>
				</script>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
				</script>
				<title>Suivi Compétition - Statistiques</title>
			</head>
			<body>
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="affProchainCombats" select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'"/>
					<xsl:with-param name="affAffectationTapis" select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'"/>
					<xsl:with-param name="affEngagements" select="/docroot/SiteConfiguration/@PublierEngagements = 'true'"/>
					<xsl:with-param name="affStatistiques" select="true()"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="selectedItem" select="'statistiques'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<div class="tas-stat-container">

					<div class="w3-container w3-blue w3-center tas-competition-bandeau">
						<div>
							<h4 class="w3-margin-0 w3-padding-small">
								<xsl:value-of select="$selectedCompetition/titre"/>
							</h4>
						</div>
						<div class="w3-card w3-indigo">
							<h5 class="w3-margin-0 w3-padding-small">
								<xsl:if test="$selectedGroupe/@sexe = 'F'">Féminines,&nbsp;</xsl:if>
								<xsl:if test="$selectedGroupe/@sexe = 'M'">Masculins,&nbsp;</xsl:if>
								<xsl:choose>
									<xsl:when test="$selectedGroupe/@type = 1">
										Nom commençant par <xsl:value-of select="$selectedGroupe/@entite"/>
									</xsl:when>
									<xsl:when test="$selectedGroupe/@type = 2">
										Club <xsl:value-of select="$RefData/structures/clubs/club[@ID = $selectedGroupe/@entite]/nom"/>
									</xsl:when>
									<xsl:when test="$selectedGroupe/@type = 3">
										Comité <xsl:value-of select="$RefData/structures/comites/comite[@ID = $selectedGroupe/@entite]/nom"/>
									</xsl:when>
									<xsl:when test="$selectedGroupe/@type = 4">
										Ligue <xsl:value-of select="$RefData/structures/ligues/ligue[@ID = $selectedGroupe/@entite]/nom"/>
									</xsl:when>
									<xsl:when test="$selectedGroupe/@type = 5 or $selectedGroupe/@type = 6">
										<xsl:value-of select="$RefData/structures/lesPays/pays[@ID = $selectedGroupe/@entite]/@nom"/>
									</xsl:when>
								</xsl:choose>
							</h5>
						</div>
					</div>

					<div>

						<xsl:if test="$selectedGroupe/@type != 1 and $selectedGroupe/StatsStructure">
							<xsl:variable name="sStat" select="$selectedGroupe/StatsStructure" />

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('club-part')">
									<img width="20" style="display: none; margin-right: 8px;" id="club-partCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="club-partExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Participation
								</button>
							</div>
							<div id="club-part" class="w3-container w3-padding-small" style="display: block;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-blue">
											<div class="tas-stat-label">
												<xsl:choose>
													<xsl:when test="$selectedGroupe/@sexe = 'F'">Inscrites</xsl:when>
													<xsl:otherwise>Inscrits</xsl:otherwise>
												</xsl:choose>
											</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@nbParticipants != ''">
														<xsl:value-of select="$sStat/@nbParticipants"/>
													</xsl:when>
													<xsl:otherwise>0</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-blue">
											<div class="tas-stat-label">
												<xsl:choose>
													<xsl:when test="$selectedGroupe/@sexe = 'F'">Pesées</xsl:when>
													<xsl:otherwise>Pesés</xsl:otherwise>
												</xsl:choose>
											</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@nbCombattants != ''">
														<xsl:value-of select="$sStat/@nbCombattants"/>
													</xsl:when>
													<xsl:otherwise>0</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-blue">
											<div class="tas-stat-label">Participation</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@pctParticipation != ''">
														<xsl:value-of select="translate($sStat/@pctParticipation, '.', ',')"/> %
													</xsl:when>
													<xsl:otherwise>0 %</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('club-res')">
									<img width="20" style="display: none; margin-right: 8px;" id="club-resCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="club-resExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Résultats
								</button>
							</div>
							<div id="club-res" class="w3-container w3-padding-small" style="display: block;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s6">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">Total Combats</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@nbCombats != ''">
														<xsl:value-of select="$sStat/@nbCombats"/>
													</xsl:when>
													<xsl:otherwise>0</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s6">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">Combats/judoka</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@nbCombats != '' and $sStat/@nbCombattants != '' and $sStat/@nbCombattants > 0">
														<xsl:value-of select="translate(format-number($sStat/@nbCombats div $sStat/@nbCombattants, '0.#'), '.', ',')"/>
													</xsl:when>
													<xsl:otherwise>0</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s6 w3-margin-top">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">% Victoires</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@pctVictoires != ''">
														<xsl:value-of select="translate($sStat/@pctVictoires, '.', ',')"/> %
													</xsl:when>
													<xsl:otherwise>0 %</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s6 w3-margin-top">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">% Hikiwake</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@pctHikiwake != ''">
														<xsl:value-of select="translate($sStat/@pctHikiwake, '.', ',')"/> %
													</xsl:when>
													<xsl:otherwise>0 %</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('club-vic')">
									<img width="20" style="display: none; margin-right: 8px;" id="club-vicCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="club-vicExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Profil des victoires
								</button>
							</div>
							<div id="club-vic" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-padding-bottom">
									<div class="w3-col m6 s12">
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Ippon direct</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireIpponDirect != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireIpponDirect, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireIpponDirect != ''">
																<xsl:value-of select="$sStat/@pctVictoireIpponDirect"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Waza-ari awasete ippon</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireWazaAriAwaseteIppon != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireWazaAriAwaseteIppon, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireWazaAriAwaseteIppon != ''">
																<xsl:value-of select="$sStat/@pctVictoireWazaAriAwaseteIppon"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Waza-ari</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireWazaAri != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireWazaAri, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireWazaAri != ''">
																<xsl:value-of select="$sStat/@pctVictoireWazaAri"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Yuko</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireYuko != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireYuko, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireYuko != ''">
																<xsl:value-of select="$sStat/@pctVictoireYuko"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
									</div>
									<div class="w3-col m6 s12">
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">3 shidos</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireSogoGachi != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireSogoGachi, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireSogoGachi != ''">
																<xsl:value-of select="$sStat/@pctVictoireSogoGachi"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Hansoku-make</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireHansokuMake != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireHansokuMake, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireHansokuMake != ''">
																<xsl:value-of select="$sStat/@pctVictoireHansokuMake"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Abandon / Forfait / Médical</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireAbandonForfaitMedical != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireAbandonForfaitMedical, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireAbandonForfaitMedical != ''">
																<xsl:value-of select="$sStat/@pctVictoireAbandonForfaitMedical"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Décision</div>
												<div class="w3-col s3 w3-right-align w3-strong">
													<xsl:choose>
														<xsl:when test="$sStat/@pctVictoireDecision != ''">
															<xsl:value-of select="translate($sStat/@pctVictoireDecision, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div class="w3-indigo tas-stat-bar">
													<xsl:attribute name="style">
														width:<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireDecision != ''">
																<xsl:value-of select="$sStat/@pctVictoireDecision"/>
															</xsl:when>
															<xsl:otherwise>0</xsl:otherwise>
														</xsl:choose>%;
													</xsl:attribute>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('club-temps')">
									<img width="20" style="display: none; margin-right: 8px;" id="club-tempsCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="club-tempsExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Durée
								</button>
							</div>
							<div id="club-temps" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Min</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="string-length($sStat/@dureeCombatMin) >= 8">
														<xsl:value-of select="substring($sStat/@dureeCombatMin, 4, 5)"/>
													</xsl:when>
													<xsl:otherwise>-</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Moyenne</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="string-length($sStat/@dureeCombatMoy) >= 8">
														<xsl:value-of select="substring($sStat/@dureeCombatMoy, 4, 5)"/>
													</xsl:when>
													<xsl:otherwise>-</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Max</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="string-length($sStat/@dureeCombatMax) >= 8">
														<xsl:value-of select="substring($sStat/@dureeCombatMax, 4, 5)"/>
													</xsl:when>
													<xsl:otherwise>-</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>

									<xsl:if test="$typeCompetition != '1'">
										<div class="w3-col s6 w3-margin-top">
											<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-amber">
												<div class="tas-stat-label">Golden Score</div>
												<div class="tas-stat-value">
													<xsl:choose>
														<xsl:when test="$sStat/@nbCombatsGoldenScore != ''">
															<xsl:value-of select="$sStat/@nbCombatsGoldenScore"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
													<span class="w3-small w3-text-grey">
														&nbsp;(<xsl:choose>
															<xsl:when test="$sStat/@pctCombatsGoldenScore != ''">
																<xsl:value-of select="translate($sStat/@pctCombatsGoldenScore, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>)
													</span>
												</div>
											</div>
										</div>
										<div class="w3-col s6 w3-margin-top">
											<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-amber">
												<div class="tas-stat-label">Moy. Golden Score</div>
												<div class="tas-stat-value">
													<xsl:choose>
														<xsl:when test="string-length($sStat/@dureeMoyenneGoldenScore) >= 8">
															<xsl:value-of select="substring($sStat/@dureeMoyenneGoldenScore, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</div>
											</div>
										</div>
									</xsl:if>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('club-discip')">
									<img width="20" style="display: none; margin-right: 8px;" id="club-discipCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="club-discipExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Discipline
								</button>
							</div>
							<div id="club-discip" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s12">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-orange">
											<div class="tas-stat-label">Moyenne de pénalités par combat</div>
											<div class="tas-stat-value">
												<xsl:choose>
													<xsl:when test="$sStat/@moyennePenalitesParCombat != ''">
														<xsl:value-of select="translate($sStat/@moyennePenalitesParCombat, '.', ',')"/>
													</xsl:when>
													<xsl:otherwise>0</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</div>
								</div>
							</div>
						</xsl:if>


						<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
							<button class="tas-stat-accordion-btn" onclick="togglePanel('club-com')">
								<img width="20" style="display: none; margin-right: 8px;" id="club-comCollapse">
									<xsl:attribute name="src">
										<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
									</xsl:attribute>
								</img>
								<img width="20" style="margin-right: 8px;" id="club-comExpand">
									<xsl:attribute name="src">
										<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
									</xsl:attribute>
								</img>
								<xsl:choose>
									<xsl:when test="$selectedGroupe/@sexe = 'F'">Combattantes</xsl:when>
									<xsl:otherwise>Combattants</xsl:otherwise>
								</xsl:choose>
							</button>
						</div>
						<div id="club-com" class="w3-container w3-padding-0" style="display: block;">

							<div class="w3-responsive w3-card w3-small w3-margin-top w3-margin-bottom">
								<table class="w3-table-all">
									<thead>
										<tr class="w3-light-blue w3-text-indigo">
											<th>Judoka</th>
											<th class="w3-center">Cbts</th>
											<th class="w3-center">Moy. Pén.</th>
											<xsl:if test="$typeCompetition != '1'">
												<th class="w3-center">% GS</th>
												<th class="w3-center">Moy. GS</th>
											</xsl:if>
											<th class="w3-center">Moy. Cbt</th>
											<th class="w3-center">% Vic.</th>
										</tr>
									</thead>
									<tbody>
										<xsl:for-each select="$selectedGroupe/judokas/judoka">
											<xsl:sort select="@nom" order="ascending"/>
											<xsl:variable name="jStat" select="StatsJudoka" />
											<xsl:variable name="sexeStr">
												<xsl:choose>
													<xsl:when test="@lib_sexe = 'F'">Féminines</xsl:when>
													<xsl:otherwise>Masculins</xsl:otherwise>
												</xsl:choose>
											</xsl:variable>

											<tr class="tas-stat-clickable-row" onclick="openJudokaStatsModal(this)">
												<xsl:attribute name="data-nom">
													<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
												</xsl:attribute>
												<xsl:attribute name="data-cat">
													<xsl:value-of select="$sexeStr"/> / <xsl:value-of select="@nom_cateage"/>
												</xsl:attribute>
												<xsl:attribute name="data-cbts">
													<xsl:choose>
														<xsl:when test="$jStat/@nbCombats != ''">
															<xsl:value-of select="$jStat/@nbCombats"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-vic">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoires != ''">
															<xsl:value-of select="$jStat/@pctVictoires"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-ippon">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireIpponDirect != ''">
															<xsl:value-of select="$jStat/@pctVictoireIpponDirect"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-wazaawa">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireWazaAriAwaseteIppon != ''">
															<xsl:value-of select="$jStat/@pctVictoireWazaAriAwaseteIppon"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-waza">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireWazaAri != ''">
															<xsl:value-of select="$jStat/@pctVictoireWazaAri"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-yuko">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireYuko != ''">
															<xsl:value-of select="$jStat/@pctVictoireYuko"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-shido3">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireSogoGachi != ''">
															<xsl:value-of select="$jStat/@pctVictoireSogoGachi"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-hansoku">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireHansokuMake != ''">
															<xsl:value-of select="$jStat/@pctVictoireHansokuMake"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-amf">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireAbandonForfaitMedical != ''">
															<xsl:value-of select="$jStat/@pctVictoireAbandonForfaitMedical"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-decision">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoireDecision != ''">
															<xsl:value-of select="$jStat/@pctVictoireDecision"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>

												<xsl:attribute name="data-tmin">
													<xsl:choose>
														<xsl:when test="string-length($jStat/@dureeCombatMin) >= 8">
															<xsl:value-of select="substring($jStat/@dureeCombatMin, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-tmoy">
													<xsl:choose>
														<xsl:when test="string-length($jStat/@dureeCombatMoy) >= 8">
															<xsl:value-of select="substring($jStat/@dureeCombatMoy, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-tmax">
													<xsl:choose>
														<xsl:when test="string-length($jStat/@dureeCombatMax) >= 8">
															<xsl:value-of select="substring($jStat/@dureeCombatMax, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>

												<xsl:attribute name="data-gscbt">
													<xsl:choose>
														<xsl:when test="$jStat/@nbCombatsGoldenScore != ''">
															<xsl:value-of select="$jStat/@nbCombatsGoldenScore"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-gspct">
													<xsl:choose>
														<xsl:when test="$jStat/@pctCombatsGoldenScore != ''">
															<xsl:value-of select="$jStat/@pctCombatsGoldenScore"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-gsmoy">
													<xsl:choose>
														<xsl:when test="string-length($jStat/@dureeMoyenneGoldenScore) >= 8">
															<xsl:value-of select="substring($jStat/@dureeMoyenneGoldenScore, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>
												<xsl:attribute name="data-pen">
													<xsl:choose>
														<xsl:when test="$jStat/@moyennePenalitesParCombat != ''">
															<xsl:value-of select="$jStat/@moyennePenalitesParCombat"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>

												<td>
													<strong>
														<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="substring(@prenom,1,1)"/>.
													</strong>
													<br/>
													<span class="w3-text-grey w3-tiny">
														<xsl:value-of select="@nom_cateage"/>
													</span>
												</td>
												<td class="w3-center">
													<xsl:choose>
														<xsl:when test="$jStat/@nbCombats != ''">
															<xsl:value-of select="$jStat/@nbCombats"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</td>
												<td class="w3-center">
													<xsl:choose>
														<xsl:when test="$jStat/@moyennePenalitesParCombat != ''">
															<xsl:value-of select="translate($jStat/@moyennePenalitesParCombat, '.', ',')"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</td>

												<xsl:if test="$typeCompetition != '1'">
													<td class="w3-center">
														<xsl:choose>
															<xsl:when test="$jStat/@pctCombatsGoldenScore != ''">
																<xsl:value-of select="translate($jStat/@pctCombatsGoldenScore, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>-</xsl:otherwise>
														</xsl:choose>
													</td>
													<td class="w3-center">
														<xsl:choose>
															<xsl:when test="string-length($jStat/@dureeMoyenneGoldenScore) >= 8">
																<xsl:value-of select="substring($jStat/@dureeMoyenneGoldenScore, 4, 5)"/>
															</xsl:when>
															<xsl:otherwise>-</xsl:otherwise>
														</xsl:choose>
													</td>
												</xsl:if>

												<td class="w3-center">
													<xsl:choose>
														<xsl:when test="string-length($jStat/@dureeCombatMoy) >= 8">
															<xsl:value-of select="substring($jStat/@dureeCombatMoy, 4, 5)"/>
														</xsl:when>
														<xsl:otherwise>-</xsl:otherwise>
													</xsl:choose>
												</td>
												<td class="w3-center">
													<xsl:choose>
														<xsl:when test="$jStat/@pctVictoires != ''">
															<xsl:value-of select="translate($jStat/@pctVictoires, '.', ',')"/> %
														</xsl:when>
														<xsl:otherwise>0 %</xsl:otherwise>
													</xsl:choose>
												</td>
											</tr>
										</xsl:for-each>
									</tbody>
								</table>
							</div>

							<div class="w3-center w3-margin-bottom w3-tiny w3-text-grey">
								<i>
									<xsl:choose>
										<xsl:when test="$selectedGroupe/@sexe = 'F'">Cliquez sur une combattante pour afficher ses statistiques détaillées.</xsl:when>
										<xsl:otherwise>Cliquez sur un combattant pour afficher ses statistiques détaillées.</xsl:otherwise>
									</xsl:choose>
								</i>
							</div>
						</div>
					</div>
				</div>

				<div id="statsModal" class="w3-modal" style="padding-top:0; z-index: 999;">
					<div class="w3-modal-content w3-animate-right tas-stat-modal-flex"
							 style="width: 100%; height: 100%; margin: 0; max-width: none; background-color: #f1f1f1;">

						<header class="w3-container w3-blue w3-padding-small tas-stat-modal-header">
							<span onclick="closeJudokaStatsModal()" class="w3-button w3-display-topright w3-xlarge w3-blue" style="padding: 4px 16px;">&times;</span>
							<h4 class="w3-margin-0" id="m-nom">-</h4>
							<div class="w3-small w3-opacity" id="m-cat">-</div>
						</header>

						<div class="tas-stat-modal-body">

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('j-res')">
									<img width="20" style="display: none; margin-right: 8px;" id="j-resCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="j-resExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Résultats
								</button>
							</div>
							<div id="j-res" class="w3-container w3-padding-small" style="display: block;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s6">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">Total Combats</div>
											<div class="tas-stat-value" id="d-combats">0</div>
										</div>
									</div>
									<div class="w3-col s6">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">% Victoires</div>
											<div class="tas-stat-value" id="d-tauxvic">0 %</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('j-vic')">
									<img width="20" style="display: none; margin-right: 8px;" id="j-vicCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="j-vicExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Profil des victoires
								</button>
							</div>
							<div id="j-vic" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-padding-bottom">
									<div class="w3-col m6 s12">
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Ippon direct</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-ippon">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-ippon" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Waza-ari awasete ippon</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-wazaawa">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-wazaawa" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Waza-ari</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-waza">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-waza" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Yuko</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-yuko">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-yuko" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
									</div>
									<div class="w3-col m6 s12">
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">3 shidos</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-shido3">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-shido3" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Hansoku-make</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-hansoku">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-hansoku" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Abandon / Forfait / Médical</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-amf">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-amf" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
										<div class="w3-margin-bottom">
											<div class="w3-row w3-small">
												<div class="w3-col s9">Décision</div>
												<div class="w3-col s3 w3-right-align w3-strong" id="lbl-decision">0 %</div>
											</div>
											<div class="w3-light-grey tas-stat-bar">
												<div id="bar-decision" class="w3-indigo tas-stat-bar" style="width:0%"></div>
											</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('j-temps')">
									<img width="20" style="display: none; margin-right: 8px;" id="j-tempsCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="j-tempsExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Durée<xsl:if test="$typeCompetition != '1'"> &amp; Golden Score</xsl:if>
								</button>
							</div>
							<div id="j-temps" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Min</div>
											<div class="tas-stat-value" id="d-tmin">-</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Moyenne</div>
											<div class="tas-stat-value" id="d-tmoy">-</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Max</div>
											<div class="tas-stat-value" id="d-tmax">-</div>
										</div>
									</div>

									<div class="w3-col s6 w3-margin-top">
										<xsl:if test="$typeCompetition = '1'">
											<xsl:attribute name="style">display:none;</xsl:attribute>
										</xsl:if>
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-amber">
											<div class="tas-stat-label">Golden Score</div>
											<div class="tas-stat-value" id="d-gscbt_pct">0 (0%)</div>
										</div>
									</div>
									<div class="w3-col s6 w3-margin-top">
										<xsl:if test="$typeCompetition = '1'">
											<xsl:attribute name="style">display:none;</xsl:attribute>
										</xsl:if>
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-amber">
											<div class="tas-stat-label">Moy. Golden Score</div>
											<div class="tas-stat-value" id="d-gsmoy">-</div>
										</div>
									</div>
								</div>
							</div>

							<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
								<button class="tas-stat-accordion-btn" onclick="togglePanel('j-discip')">
									<img width="20" style="display: none; margin-right: 8px;" id="j-discipCollapse">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
										</xsl:attribute>
									</img>
									<img width="20" style="margin-right: 8px;" id="j-discipExpand">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
										</xsl:attribute>
									</img>
									Discipline
								</button>
							</div>
							<div id="j-discip" class="w3-container w3-padding-small" style="display: none;">
								<div class="w3-row-padding w3-margin-top w3-margin-bottom">
									<div class="w3-col s12">
										<div class="w3-card w3-white w3-padding-small w3-round-small tas-stat-card tas-stat-border-orange">
											<div class="tas-stat-label">Moyenne de pénalités par combat</div>
											<div class="tas-stat-value" id="d-pen">0.0</div>
										</div>
									</div>
								</div>
							</div>

						</div>
					</div>
				</div>

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
</xsl:stylesheet>