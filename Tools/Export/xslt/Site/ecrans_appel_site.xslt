<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Tools/Export/xslt/Site/niveau_tour_combat.xslt"/>

	<xsl:output method="html" indent="yes" encoding="utf-8"/>

	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>

	<xsl:param name="tailleGroupe"/>
	<xsl:param name="idEcran"/>
	<xsl:param name="tapisAffiches"/>
	<xsl:param name="dispositionAffichage" select="'colonne'"/>
	<xsl:param name="combatsParPageEff"/>
	<xsl:param name="isAffichageCombatLigne" select="'false'"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable name="docPrincipal" select="/" />

	<xsl:variable select="/docroot/SiteConfiguration/@DelaiDeroulementSec" name="delaiDeroulementSec"/>
	<xsl:variable select="number(/docroot/SiteConfiguration/@NbProchainsCombats)" name="nbProchainsCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@urlRedirecteur" name="urlRedirecteur"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DateGeneration" name="dateGeneration"/>
	<xsl:variable select="/docroot/SiteConfiguration/@AppVersion" name="appVersion"/>

	<xsl:variable name="couleur1" select="/docroot/competition/@couleur1" />
	<xsl:variable name="couleur2" select="/docroot/competition/@couleur2" />
	<xsl:variable name="idCompetition" select="/docroot/competition/@ID" />
	<xsl:variable name="typeCompetition" select="/docroot/competition/@type" />
	<xsl:variable name="TitreCompetition" select="/docroot/competition/titre"/>

	<xsl:variable name="nbProchainsCombatsEff">
		<xsl:choose>
			<xsl:when test="$nbProchainsCombats > 0">
				<xsl:value-of select="$nbProchainsCombats"/>
			</xsl:when>
			<xsl:otherwise>10</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="maxCombatsPage">
		<xsl:choose>
			<xsl:when test="number($combatsParPageEff) > 10">10</xsl:when>
			<xsl:when test="number($combatsParPageEff) > 0">
				<xsl:value-of select="$combatsParPageEff"/>
			</xsl:when>
			<xsl:otherwise>8</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="widthStyle">
		<xsl:choose>
			<xsl:when test="$tailleGroupe = '1'">100%</xsl:when>
			<xsl:when test="$tailleGroupe = '2' and $dispositionAffichage = 'ligne'">100%</xsl:when>
			<xsl:otherwise>50%</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="heightStyle">
		<xsl:choose>
			<xsl:when test="$tailleGroupe = '1'">calc(100vh - 10.5vh)</xsl:when>
			<xsl:when test="$tailleGroupe = '2' and $dispositionAffichage = 'ligne'">calc((100vh - 10.5vh) / 2)</xsl:when>
			<xsl:when test="$tailleGroupe = '2'">calc(100vh - 10.5vh)</xsl:when>
			<xsl:otherwise>calc((100vh - 10.5vh) / 2)</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

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
						<xsl:value-of select="concat($cssPath, 'style-ecran-appel.css')"/>
					</xsl:attribute>
				</link>

				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-animation.js')"/>
					</xsl:attribute>
				</script>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
				</script>
				<title>
					Ecran Appel - <xsl:value-of select="$TitreCompetition"/>
				</title>
			</head>

			<body class="w3-black w3-sans-serif tv-body">
				<!-- La ligne d'entete -->
				<div class="tv-header w3-white w3-card w3-cell-row">
					<!-- Logo de la compétition	-->
					<div class="w3-cell w3-cell-middle tv-logo-cell">
						<img alt="Logo" class="tv-logo" onerror="this.style.display='none'">
							<xsl:attribute name="src">
								<xsl:value-of select="concat($imgPath, $logo)"/>
							</xsl:attribute>
						</img>
					</div>
					
					<!-- Titre de la compétition -->
					<div class="tv-title w3-cell w3-cell-middle w3-xxlarge w3-text-indigo">
						<xsl:value-of select="$TitreCompetition"/>
					</div>
					
					<!-- La version -->
					<div class="w3-cell w3-cell-middle w3-right-align w3-opacity w3-padding-small" style="width: 15%; font-size: 1.5vh;">
						v<xsl:value-of select="$appVersion"/>
					</div>
				</div>

				<!-- Le contenu de la page -->
				<div id="main-container" class="main-container-flex"
					 data-layout-mode="{$tailleGroupe}"
					 data-duree-rotation="{$delaiDeroulementSec}"
					 data-combats-par-page="{$maxCombatsPage}"
					 data-url-redirecteur="{$urlRedirecteur}">
					<xsl:for-each select="$tapisAffiches/tapisIds/tapis">
						<xsl:variable name="numTapis" select="@id" />
						<xsl:call-template name="UnTapis">
							<xsl:with-param name="notapis" select="$numTapis"/>
							<xsl:with-param name="Position" select="position()"/>
						</xsl:call-template>
					</xsl:for-each>
				</div>

				<!-- La barre de progression -->
				<div id="progress-container" class="progress-container">
					<div id="progress-bar" class="progress-bar w3-olive"></div>
					<div class="progress-text">
						Mise à jour : <xsl:value-of select="$dateGeneration"/>
					</div>
				</div>

				<script type="text/javascript">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-animation.js')"/>
					</xsl:attribute>
				</script>

			</body>
		</html>
	</xsl:template>

	<xsl:template name="UnTapis">
		<xsl:param name="notapis"/>
		<xsl:param name="Position"/>

		<xsl:variable name="pageIndex" select="floor(($Position - 1) div $tailleGroupe) + 1" />

		<div id="tapis_{$notapis}"
					 class="tapis-card w3-animate-opacity"
					 data-tapis-page="{$pageIndex}"
					 data-tapis-numero="{$notapis}"
					 style="display:none; width: {$widthStyle}; height: {$heightStyle};">

			<div class="w3-padding-small tapis-card-inner-wrapper">
				<div>
					<xsl:attribute name="class">
						<xsl:text>tapis-inner w3-white w3-round-large w3-card-4 </xsl:text>
						<xsl:choose>
							<xsl:when test="$dispositionAffichage = 'ligne'">dispo-ligne</xsl:when>
							<xsl:otherwise>dispo-colonne</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>

					<xsl:choose>
						<xsl:when test="$dispositionAffichage = 'ligne'">
							<div class="tapis-header w3-indigo w3-display-container tapis-header-ligne">
								<div class="w3-display-topmiddle w3-center paging-ligne-container">
									<div id="paging_indicator_tapis_{$notapis}" class="paging-ligne-dots"></div>
								</div>
								<b class="w3-xxlarge">
									Tapis <xsl:value-of select="$notapis"/>
								</b>
							</div>
						</xsl:when>

						<xsl:otherwise>
							<div class="tapis-header w3-indigo w3-center w3-display-container w3-padding w3-xxlarge tapis-header-colonne">
								<div class="w3-display-topright w3-padding w3-large paging-colonne-container">
									<div id="paging_indicator_tapis_{$notapis}" class="paging-colonne-dots"></div>
								</div>
								<b>
									Tapis <xsl:value-of select="$notapis"/>
								</b>
							</div>
						</xsl:otherwise>
					</xsl:choose>

					<div class="tapis-content">
						<table class="combat-list w3-table w3-striped combat-table">
							<tbody id="liste_combats_tapis_{$notapis}">
								<xsl:for-each select="$docPrincipal//tapis/combats/combat[ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]">
									<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>
									<xsl:call-template name="UnCombat">
										<xsl:with-param name="combat" select="."/>
										<xsl:with-param name="indexCombat" select="position()"/>
									</xsl:call-template>
								</xsl:for-each>
								<xsl:if test="count($docPrincipal//tapis/combats/combat[ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]) = 0">
									<tr>
										<td colspan="4" class="w3-center w3-padding-large w3-text-grey w3-xlarge">
											<i>Aucun combat en attente sur ce tapis</i>
										</td>
									</tr>
								</xsl:if>
								<tr class="ghost-row">
									<td colspan="4" class="ghost-cell"></td>
								</tr>
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template name="UnCombat">
		<xsl:param name="combat"/>
		<xsl:param name="indexCombat"/>

		<xsl:variable name="epreuve" select="$combat/@epreuve"/>
		<xsl:variable name="phase" select="$combat/@phase"/>
		<xsl:variable name="typePhase" select="$docPrincipal//phase[@id = $phase]/@typePhase"/>

		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="$combat/ancestor::tapis[1]/participants/participant[@judoka = $participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$judoka1/@club"/>
		<xsl:variable name="comite1" select="$docPrincipal//club[@ID = $club1]/@comite"/>
		<xsl:variable name="ligue1" select="$docPrincipal//club[@ID = $club1]/@ligue"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="$combat/ancestor::tapis[1]/participants/participant[@judoka = $participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$judoka2/@club"/>
		<xsl:variable name="comite2" select="$docPrincipal//club[@ID = $club2]/@comite"/>
		<xsl:variable name="ligue2" select="$docPrincipal//club[@ID = $club2]/@ligue"/>

		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">w3-blue colorized-img-white</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">w3-purple colorized-img-white</xsl:when>
				<xsl:otherwise>w3-lime colorized-img-black</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="rowClass">
			<xsl:choose>
				<xsl:when test="$indexCombat = 1">combat-row w3-pale-green</xsl:when>
				<xsl:otherwise>combat-row</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="rowHeightPct" select="100 div $maxCombatsPage" />

		<tr id="combat_{$combat/@id}" data-row-index="{$indexCombat}" class="{$rowClass}">
			<xsl:attribute name="style">
				<xsl:text>height: </xsl:text>
				<xsl:value-of select="$rowHeightPct"/>
				<xsl:text>%;</xsl:text>
			</xsl:attribute>

			<td class="w3-center w3-cell-middle badge-cell">
				<div class="w3-indigo w3-circle pos-badge">
					<xsl:value-of select="$indexCombat"/>
				</div>
			</td>

			<td class="w3-cell-middle judoka-cell">
				<div>
					<xsl:attribute name="class">
						<xsl:text>judoka-box </xsl:text>
						<xsl:choose>
							<xsl:when test="$participant1 = 'null'">w3-sand w3-card w3-round-small w3-center</xsl:when>
							<xsl:otherwise>
								<xsl:choose>
									<xsl:when test="$couleur1 = 'Bleu'">w3-blue w3-card w3-round-small w3-right-align</xsl:when>
									<xsl:when test="$couleur1 = 'Rouge'">w3-red w3-card w3-round-small w3-right-align</xsl:when>
									<xsl:otherwise>w3-grey w3-card w3-round-small w3-right-align</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>

					<xsl:choose>
						<xsl:when test="$participant1 = 'null'">
							<div>
								<xsl:attribute name="class">
									<xsl:text>w3-container </xsl:text>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-attente-ligne</xsl:when>
										<xsl:otherwise>jc-attente-colonne</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<div class="w3-xlarge txt-bold">
									<img class="img img-attente" width="28">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
										</xsl:attribute>
									</img>
									<span class="txt-attente">En Attente</span>
								</div>
								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-medium </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">hide-ligne</xsl:when>
											<xsl:otherwise>club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
								</div>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="ecartement1" select="$docPrincipal//phase[@id = $phase]/@ecartement"/>
							<div>
								<xsl:attribute name="class">
									<xsl:text>w3-container </xsl:text>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-normal-ligne-right</xsl:when>
										<xsl:otherwise>jc-normal-colonne-right</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>

								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-xlarge </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-2</xsl:when>
											<xsl:otherwise>order-1</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<b>
										<xsl:value-of select="$judoka1/@nom"/>
									</b>
									<xsl:if test="$typeCompetition != 1">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka1/@prenom"/>
									</xsl:if>
								</div>

								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-medium w3-opacity-min club-base </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-1 club-ligne-right</xsl:when>
											<xsl:otherwise>order-2 club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<xsl:choose>
										<xsl:when test="$ecartement1 = '3'">
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club1]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$comite1"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$comite1"/>
											</xsl:if>
										</xsl:when>
										<xsl:when test="$ecartement1 = '4'">
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club1]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$docPrincipal//ligue[@ID = $ligue1]/nomCourt"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$docPrincipal//ligue[@ID = $ligue1]/nomCourt"/>
											</xsl:if>
										</xsl:when>
										<xsl:otherwise>
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club1]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$comite1"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$comite1"/>
											</xsl:if>
										</xsl:otherwise>
									</xsl:choose>
								</div>
							</div>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>

			<td class="w3-center cat-cell">
				<div class="w3-card w3-pale-yellow w3-round-small w3-large cat-box">
					<div>
						<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@sexe"/>
						<xsl:text>&#32;</xsl:text>
						<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@nom"/>
					</div>

					<div class="w3-opacity w3-medium cat-subtitle">
						(<xsl:call-template name="NiveauTourCombat">
							<xsl:with-param name="combat" select="$combat"/>
							<xsl:with-param name="typePhase" select="$typePhase"/>
							<xsl:with-param name="repechage" select="$combat/feuille/@repechage = 'true'"/>
						</xsl:call-template>)
					</div>

					<xsl:if test="$typeCompetition = 1">
						<div class="w3-margin-top">
							<xsl:attribute name="class">
								w3-tag w3-round-large w3-small <xsl:value-of select="$firstrencontreclass"/>
							</xsl:attribute>
							<img class="img" width="16">
								<xsl:attribute name="src">
									<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
								</xsl:attribute>
							</img>
							<xsl:text> </xsl:text>
							<xsl:value-of select="$combat/@firstrencontrelib"/>
						</div>
					</xsl:if>
				</div>
			</td>

			<td class="w3-cell-middle judoka-cell">
				<div>
					<xsl:attribute name="class">
						<xsl:text>judoka-box </xsl:text>
						<xsl:choose>
							<xsl:when test="$participant2 = 'null'">w3-sand w3-card w3-round-small w3-center</xsl:when>
							<xsl:otherwise>
								<xsl:choose>
									<xsl:when test="$couleur2 = 'Bleu'">w3-blue w3-card w3-round-small w3-left-align</xsl:when>
									<xsl:when test="$couleur2 = 'Rouge'">w3-red w3-card w3-round-small w3-left-align</xsl:when>
									<xsl:otherwise>w3-light-grey w3-card w3-round-small w3-left-align</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>

					<xsl:choose>
						<xsl:when test="$participant2 = 'null'">
							<div>
								<xsl:attribute name="class">
									<xsl:text>w3-container </xsl:text>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-attente-ligne</xsl:when>
										<xsl:otherwise>jc-attente-colonne</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<div class="w3-xlarge txt-bold">
									<img class="img img-attente" width="28">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
										</xsl:attribute>
									</img>
									<span class="txt-attente">En Attente</span>
								</div>
								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-medium </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">hide-ligne</xsl:when>
											<xsl:otherwise>club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
								</div>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="ecartement2" select="$docPrincipal//phase[@id = $phase]/@ecartement"/>
							<div>
								<xsl:attribute name="class">
									<xsl:text>w3-container </xsl:text>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-normal-ligne-left</xsl:when>
										<xsl:otherwise>jc-normal-colonne-left</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>

								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-xlarge order-1</xsl:text>
									</xsl:attribute>
									<b>
										<xsl:value-of select="$judoka2/@nom"/>
									</b>
									<xsl:if test="$typeCompetition != 1">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka2/@prenom"/>
									</xsl:if>
								</div>

								<div>
									<xsl:attribute name="class">
										<xsl:text>w3-medium w3-opacity-min club-base </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-2 club-ligne-left</xsl:when>
											<xsl:otherwise>order-2 club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<xsl:choose>
										<xsl:when test="$ecartement2 = '3'">
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club2]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$comite2"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$comite2"/>
											</xsl:if>
										</xsl:when>
										<xsl:when test="$ecartement2 = '4'">
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club2]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$docPrincipal//ligue[@ID = $ligue2]/nomCourt"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$docPrincipal//ligue[@ID = $ligue2]/nomCourt"/>
											</xsl:if>
										</xsl:when>
										<xsl:otherwise>
											<xsl:if test="$typeCompetition != '1'">
												<xsl:value-of select="$docPrincipal//club[@ID = $club2]/nomCourt"/>
												<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
												<xsl:value-of select="$comite2"/>
											</xsl:if>
											<xsl:if test="$typeCompetition = '1'">
												<xsl:value-of select="$comite2"/>
											</xsl:if>
										</xsl:otherwise>
									</xsl:choose>
								</div>
							</div>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>