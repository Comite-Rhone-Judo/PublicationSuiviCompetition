<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
	<!ENTITY nl "&#10;">
]>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="idgroupe"/>
	<xsl:param name="idcompetition"/>
	<xsl:param name="RefData"/>
	<xsl:param name="SiteRoutes"/>

	<xsl:variable name="imgPath" select="$SiteRoutes/@urlImg"/>
	<xsl:variable name="jsPath" select="$SiteRoutes/@urlJs"/>
	<xsl:variable name="cssPath" select="$SiteRoutes/@urlCss"/>
	<xsl:variable name="commonPath" select="$SiteRoutes/*/@UrlCommon"/>
	<!-- Récupération de l'URL des statistiques pour CE groupe -->
	<xsl:variable name="urlEngagements" select="$SiteRoutes/routeGroupe[@groupe = $idgroupe and @typeGroupe = 'engagement']/@urlGroupe" />


	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

	<xsl:variable name="selectedCompetition" select="/docroot/competitions/competition[@ID = $idcompetition]"/>
	<xsl:variable select="$selectedCompetition/@type" name="typeCompetition"/>
	<xsl:variable select="$selectedCompetition/@niveau" name="niveauCompetition"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

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

				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'w3.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-common.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-statistiques.css')}"/>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
					gDefaultAutoReload = <xsl:value-of select="$actualisationClientDefaut"/>;
				</script>

				<script src="{concat($jsPath, 'site-display.js')}"/>

				<title>Suivi Compétition - Statistiques</title>
			</head>
			<body>
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
					<xsl:with-param name="affProchainCombats" select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'"/>
					<xsl:with-param name="affAffectationTapis" select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'"/>
					<xsl:with-param name="affEngagements" select="/docroot/SiteConfiguration/@PublierEngagements = 'true'"/>
					<xsl:with-param name="affStatistiques" select="true()"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="selectedItem" select="'statistiques'"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>

				<div class="tas-stat-container">

					<!-- Bandeau Modernisé -->
					<div class="tas-competition-bandeau">
						<div>
							<h4>
								<xsl:value-of select="$selectedCompetition/titre"/>
							</h4>
							<h5 class="tas-groupe-titre-container">
								<span>
									<xsl:if test="$selectedGroupe/@sexe = 'F'">Féminines,&nbsp;</xsl:if>
									<xsl:if test="$selectedGroupe/@sexe = 'M'">Masculins,&nbsp;</xsl:if>
									<xsl:call-template name="LibelleGroupeStructure">
										<xsl:with-param name="typeGroupe" select="$selectedGroupe/@type"/>
										<xsl:with-param name="niveauCompetition" select="$niveauCompetition"/>
										<xsl:with-param name="entiteId" select="$selectedGroupe/@entite"/>
										<xsl:with-param name="RefData" select="$RefData"/>
										<xsl:with-param name="avecPrefixe" select="'true'"/>
									</xsl:call-template>
								</span>

								<!-- Bouton d'accès aux Engagements -->
								<xsl:if test="$urlEngagements != ''">
									<a href="{$urlEngagements}" class="w3-button w3-circle tas-icon-btn tas-btn-statistiques" title="Voir les engagements de ce groupe">
										<img class="tas-theme-icon" src="{$imgPath}list_ingredients-32.png" width="20" />
									</a>
								</xsl:if>
							</h5>
						</div>
					</div>

					<div class="w3-padding-small">

						<xsl:if test="$selectedGroupe/@type != 1 and $selectedGroupe/StatsStructure">
							<xsl:variable name="sStat" select="$selectedGroupe/StatsStructure" />

							<!-- Bloc PARTICIPATION -->
							<button class="ios-accordion-btn" onclick="togglePanel('club-part')">
								<span>Participation</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="club-partCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="club-partExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="club-part" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-club-partExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-part')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-club-part" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-club-part')" class="tas-callout-close">&times;</button>
									<div>
										<strong>Inscrits :</strong> Enregistrés à la compétition.<br/>
										<strong>Présents :</strong> Ayant passé la pesée.<br/>
										<strong>Participation :</strong> Taux de présence effectif.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s4">
										<div class="ios-card tas-stat-card tas-stat-border-blue">
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
										<div class="ios-card tas-stat-card tas-stat-border-blue">
											<div class="tas-stat-label">
												<xsl:choose>
													<xsl:when test="$selectedGroupe/@sexe = 'F'">Présentes</xsl:when>
													<xsl:otherwise>Présents</xsl:otherwise>
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
										<div class="ios-card tas-stat-card tas-stat-border-blue">
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

							<!-- Bloc RESULTATS -->
							<button class="ios-accordion-btn" onclick="togglePanel('club-res')">
								<span>Résultats</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="club-resCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="club-resExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="club-res" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-club-resExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-res')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-club-res" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-club-res')" class="tas-callout-close">&times;</button>
									<div>
										<strong>Total combats :</strong> Cumul du nombre de combats disputés par les combattants du groupe.<br/>
										<strong>Combats/judoka :</strong> Nombre moyen de combats disputés par combattant du groupe.<br/>
										<strong>% Victoire :</strong> Pourcentage de victoire par combattant du groupe.<br/>
										<strong>Hikiwake :</strong> Pourcentage de matchs nuls.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s6">
										<div class="ios-card tas-stat-card tas-stat-border-green">
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
										<div class="ios-card tas-stat-card tas-stat-border-green">
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
										<div class="ios-card tas-stat-card tas-stat-border-green">
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
										<div class="ios-card tas-stat-card tas-stat-border-green">
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

							<!-- Bloc PROFILS VICTOIRES -->
							<button class="ios-accordion-btn" onclick="togglePanel('club-vic')">
								<span>Profil des victoires</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="club-vicCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="club-vicExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="club-vic" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-club-vicExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-vic')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-club-vic" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-club-vic')" class="tas-callout-close">&times;</button>
									<div>
										Répartition de toutes les victoires du groupe selon <strong>l'avantage décisif marqué</strong> (Ippon, Waza-ari...) ou la <strong>sanction de l'adversaire</strong> (Hansoku-make, Shidos, forfait).
									</div>
								</div>

								<div class="ios-card w3-padding w3-margin-top">
									<div class="w3-row-padding">
										<div class="w3-col m6 s12">
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Ippon direct</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireIpponDirect != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireIpponDirect, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireIpponDirect != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireIpponDirect)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Waza-ari awasete ippon</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireWazaAriAwaseteIppon != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireWazaAriAwaseteIppon, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireWazaAriAwaseteIppon != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireWazaAriAwaseteIppon)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Waza-ari</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireWazaAri != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireWazaAri, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireWazaAri != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireWazaAri)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Yuko</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireYuko != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireYuko, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireYuko != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireYuko)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
										</div>
										<div class="w3-col m6 s12">
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">3 shidos</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireSogoGachi != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireSogoGachi, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireSogoGachi != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireSogoGachi)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Hansoku-make</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireHansokuMake != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireHansokuMake, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireHansokuMake != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireHansokuMake)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Abandon / Forfait / Médical</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireAbandonForfaitMedical != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireAbandonForfaitMedical, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireAbandonForfaitMedical != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireAbandonForfaitMedical)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Décision</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;">
														<xsl:choose>
															<xsl:when test="$sStat/@pctVictoireDecision != ''">
																<xsl:value-of select="translate($sStat/@pctVictoireDecision, '.', ',')"/> %
															</xsl:when>
															<xsl:otherwise>0 %</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div class="tas-stat-bar fj-bg-blue">
														<xsl:attribute name="style">
															<xsl:choose>
																<xsl:when test="$sStat/@pctVictoireDecision != ''">
																	width:<xsl:value-of select="normalize-space($sStat/@pctVictoireDecision)"/>%;
																</xsl:when>
																<xsl:otherwise>width:0%;</xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>

							<!-- Bloc DUREE -->
							<button class="ios-accordion-btn" onclick="togglePanel('club-temps')">
								<span>Durée</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="club-tempsCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="club-tempsExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="club-temps" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-club-tempsExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-temps')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-club-temps" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-club-temps')" class="tas-callout-close">&times;</button>
									<div>
										Temps de combat effectif (minimum, moyen, maximum) du groupe.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s4">
										<div class="ios-card tas-stat-card tas-stat-border-teal">
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
										<div class="ios-card tas-stat-card tas-stat-border-teal">
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
										<div class="ios-card tas-stat-card tas-stat-border-teal">
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
								</div>
							</div>

							<!-- Bloc DISCIPLINE -->
							<button class="ios-accordion-btn" onclick="togglePanel('club-discip')">
								<span>Discipline</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="club-discipCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="club-discipExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="club-discip" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-club-discipExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-discip')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-club-discip" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-club-discip')" class="tas-callout-close">&times;</button>
									<div>
										<strong>Moyenne de pénalités :</strong> Nombre moyen de Shidos reçus par combat pour les membres de ce groupe.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s12">
										<div class="ios-card tas-stat-card tas-stat-border-orange">
											<div class="tas-stat-label">Moyenne de shidos</div>
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

						<!-- Bloc COMBATTANTS -->
						<button class="ios-accordion-btn" onclick="togglePanel('club-com')">
							<span>
								<xsl:choose>
									<xsl:when test="$selectedGroupe/@sexe = 'F'">Combattantes</xsl:when>
									<xsl:otherwise>Combattants</xsl:otherwise>
								</xsl:choose>
							</span>
							<div>
								<img class="tas-accordion-icon tas-icon-hidden" id="club-comCollapse" src="{$imgPath}up_circular-32.png"/>
								<img class="tas-accordion-icon tas-icon-visible" id="club-comExpand" src="{$imgPath}down_circular-32.png"/>
							</div>
						</button>

						<div id="club-com" class="tasClosedPanelType tas-accordion-content-hidden">
							<div class="w3-right-align tas-info-btn-container">
								<button id="info-bloc-club-comExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-club-com')">
									<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
								</button>
							</div>

							<div id="info-bloc-club-com" class="tasClosedPanelType tas-callout" style="display: none;">
								<button onclick="togglePanel('info-bloc-club-com')" class="tas-callout-close">&times;</button>
								<div>
									<strong>Cbts :</strong> Total des combats<br/>
									<strong>Moy. Pén. :</strong> Moyenne de shidos<br/>
									<strong>Moy. Cbt :</strong> Durée moyenne des combats<br/>
									<strong>% Vic. :</strong> Taux de victoires
								</div>
							</div>

							<div class="ios-card w3-responsive w3-margin-top w3-margin-bottom" style="border-radius: 8px;">
								<table class="w3-table w3-bordered w3-small" style="background: transparent;">
									<thead style="background-color: var(--bg-color); color: var(--text-color);">
										<tr>
											<th>Judoka</th>
											<th class="w3-center">Cbts</th>
											<th class="w3-center">Moy. Pén.</th>
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

											<xsl:variable name="clubNode" select="$RefData/structures/clubs/club[@ID = current()/@club]"/>
											<xsl:variable name="comiteNode" select="$RefData/structures/comites/comite[@ID = $clubNode/@comite]"/>
											<xsl:variable name="ligueNode" select="$RefData/structures/ligues/ligue[@ID = $comiteNode/@ligue]"/>
											<xsl:variable name="paysNode" select="$RefData/structures/lesPays/pays[@ID = current()/@pays]"/>

											<xsl:variable name="structureLibelle">
												<xsl:call-template name="LibelleStructure">
													<xsl:with-param name="ecartement" select="$niveauCompetition" />
													<xsl:with-param name="typeCompetition" select="$typeCompetition" />
													<xsl:with-param name="club" select="$clubNode/nomCourt" />
													<xsl:with-param name="comite" select="$comiteNode/@ID" />
													<xsl:with-param name="ligue" select="$ligueNode/nomCourt" />
													<xsl:with-param name="pays" select="$paysNode/@abr3" />
												</xsl:call-template>
											</xsl:variable>

											<tr class="tas-stat-clickable-row" onclick="openJudokaStatsModal(this)">
												<xsl:attribute name="data-id">
													<xsl:value-of select="@id"/>
												</xsl:attribute>
												<xsl:attribute name="data-nom">
													<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
												</xsl:attribute>
												<xsl:attribute name="data-cat">
													<xsl:value-of select="$sexeStr"/> / <xsl:value-of select="@libepreuve"/>
												</xsl:attribute>
												<xsl:attribute name="data-club">
													<xsl:value-of select="$structureLibelle"/>
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
												<xsl:attribute name="data-pen">
													<xsl:choose>
														<xsl:when test="$jStat/@moyennePenalitesParCombat != ''">
															<xsl:value-of select="$jStat/@moyennePenalitesParCombat"/>
														</xsl:when>
														<xsl:otherwise>0</xsl:otherwise>
													</xsl:choose>
												</xsl:attribute>

												<td class="tas-compact-cell">
													<div>
														<strong>
															<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="substring(@prenom,1,1)"/>.
														</strong>
													</div>
													<div class="text-muted w3-tiny">
														<xsl:value-of select="$structureLibelle"/>
													</div>
													<div class="w3-text-grey w3-tiny">
														<xsl:value-of select="@libepreuve"/>
													</div>
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

							<div class="w3-center w3-margin-bottom w3-tiny text-muted">
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

				<!-- MODALE JUDOKA INDIVIDUEL -->
				<div id="statsModal" class="w3-modal" style="padding-top:0; z-index: 999;">
					<div class="w3-modal-content w3-animate-right tas-stat-modal-flex">

						<header class="tas-stat-modal-header">
							<button onclick="closeJudokaStatsModal()" class="w3-button w3-display-topright w3-xlarge" style="background: transparent; padding: 4px 16px;">&times;</button>
							<h4 class="tas-margin-none" id="m-nom" style="font-weight: 600;">-</h4>
							<div class="w3-tiny text-muted" id="m-club">-</div>
							<div class="w3-tiny text-muted" id="m-cat">-</div>
						</header>

						<div class="tas-stat-modal-body">

							<!-- Bloc RESULTATS MODALE -->
							<button class="ios-accordion-btn" onclick="togglePanel('j-res')">
								<span>Résultats</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="j-resCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="j-resExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="j-res" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-j-resExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-j-res')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-j-res" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-j-res')" class="tas-callout-close">&times;</button>
									<div>
										<strong>Total Combats :</strong> Nombre de matchs disputés.<br/>
										<strong>% Victoires :</strong> Ratio des matchs remportés.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s6">
										<div class="ios-card tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">Total Combats</div>
											<div class="tas-stat-value" id="d-combats">0</div>
										</div>
									</div>
									<div class="w3-col s6">
										<div class="ios-card tas-stat-card tas-stat-border-green">
											<div class="tas-stat-label">% Victoires</div>
											<div class="tas-stat-value" id="d-tauxvic">0 %</div>
										</div>
									</div>
								</div>
							</div>

							<!-- Bloc PROFILS VICTOIRES MODALE -->
							<button class="ios-accordion-btn" onclick="togglePanel('j-vic')">
								<span>Profil des victoires</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="j-vicCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="j-vicExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="j-vic" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-j-vicExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-j-vic')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-j-vic" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-j-vic')" class="tas-callout-close">&times;</button>
									<div>
										Proportion des victoires du combattant selon <strong>l'avantage décisif marqué</strong> (ex: Ippon) ou la <strong>sanction de l'adversaire</strong> (ex: Hansoku-make).
									</div>
								</div>

								<div class="ios-card w3-padding w3-margin-top">
									<div class="w3-row-padding">
										<div class="w3-col m6 s12">
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Ippon direct</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-ippon">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-ippon" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Waza-ari awasete ippon</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-wazaawa">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-wazaawa" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Waza-ari</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-waza">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-waza" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Yuko</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-yuko">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-yuko" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
										</div>
										<div class="w3-col m6 s12">
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">3 shidos</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-shido3">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-shido3" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Hansoku-make</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-hansoku">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-hansoku" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Abandon / Forfait / Médical</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-amf">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-amf" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
											<div class="w3-margin-bottom">
												<div class="w3-row w3-small" style="margin-bottom: 2px;">
													<div class="w3-col s9">Décision</div>
													<div class="w3-col s3 w3-right-align" style="font-weight: 600;" id="lbl-decision">0 %</div>
												</div>
												<div class="tas-stat-bar" style="background-color: var(--border-color);">
													<div id="bar-decision" class="tas-stat-bar fj-bg-blue" style="width:0%"></div>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>

							<!-- Bloc DUREE MODALE -->
							<button class="ios-accordion-btn" onclick="togglePanel('j-temps')">
								<span>Durée</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="j-tempsCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="j-tempsExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="j-temps" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-j-tempsExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-j-temps')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-j-temps" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-j-temps')" class="tas-callout-close">&times;</button>
									<div>
										Temps de combat effectif (minimum, moyen, maximum) du judoka.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s4">
										<div class="ios-card tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Min</div>
											<div class="tas-stat-value" id="d-tmin">-</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="ios-card tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Moyenne</div>
											<div class="tas-stat-value" id="d-tmoy">-</div>
										</div>
									</div>
									<div class="w3-col s4">
										<div class="ios-card tas-stat-card tas-stat-border-teal">
											<div class="tas-stat-label">Max</div>
											<div class="tas-stat-value" id="d-tmax">-</div>
										</div>
									</div>
								</div>
							</div>

							<!-- Bloc DISCIPLINE MODALE -->
							<button class="ios-accordion-btn" onclick="togglePanel('j-discip')">
								<span>Discipline</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="j-discipCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="j-discipExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>

							<div id="j-discip" class="tasClosedPanelType tas-accordion-content-hidden">
								<div class="w3-right-align tas-info-btn-container">
									<button id="info-bloc-j-discipExpand" class="w3-button w3-transparent tas-info-btn" onclick="togglePanel('info-bloc-j-discip')">
										<img width="18" alt="Info" src="{$imgPath}info-32.png" class="tas-theme-icon"/>
									</button>
								</div>

								<div id="info-bloc-j-discip" class="tasClosedPanelType tas-callout" style="display: none;">
									<button onclick="togglePanel('info-bloc-j-discip')" class="tas-callout-close">&times;</button>
									<div>
										<strong>Moyenne :</strong> Nombre moyen de Shidos reçus.
									</div>
								</div>

								<div class="w3-row-padding">
									<div class="w3-col s12">
										<div class="ios-card tas-stat-card tas-stat-border-orange">
											<div class="tas-stat-label">Moyenne de shidos</div>
											<div class="tas-stat-value" id="d-pen">0.0</div>
										</div>
									</div>
								</div>
							</div>

						</div>
					</div>
				</div>

				<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
					<script src="{concat($jsPath, 'footer_script.js')}"/>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>