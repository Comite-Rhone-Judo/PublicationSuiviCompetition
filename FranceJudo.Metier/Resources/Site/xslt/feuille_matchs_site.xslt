<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/niveau_tour_combat.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="istapis"/>
	<xsl:param name="useIntituleCommun"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>
	<xsl:param name="RefData"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable name="couleur1" select="/docroot/competition/@couleur1"/>
	<xsl:variable name="couleur2" select="/docroot/competition/@couleur2"/>
	<xsl:variable name="idCompetition" select="/docroot/competition/@ID"/>
	<xsl:variable name="typeCompetition" select="/docroot/competition/@type"/>
	<xsl:variable name="niveauCompetition" select="/docroot/competition/@niveau"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="number(/docroot/SiteConfiguration/@NbProchainsCombats)" name="nbProchainsCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@MsgProchainsCombats" name="msgProchainsCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:variable name="nbProchainsCombatsEff">
		<xsl:choose>
			<xsl:when test="$nbProchainsCombats > 0">
				<xsl:value-of select="$nbProchainsCombats"/>
			</xsl:when>
			<xsl:otherwise>6</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<!-- Affiche les details de la competition si on est en judo avec plus d'une competition -->
	<xsl:variable select="count(//epreuve[@competition!=$idCompetition])!=0 and /docroot/competition/@disciplineId = 1" name="affDetailCompetition"/>

	<!-- En jujitsu, en affiche la discipline -->
	<xsl:variable select="/docroot/competition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

	<xsl:variable name="selectedItemName">
		<xsl:choose>
			<xsl:when test="$istapis = 'alltapis'">se_prepare</xsl:when>
			<xsl:otherwise>prochains_combats</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<META http-equiv="Content-Type" content="text/html; charset=utf-8"/>
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

				<!-- Feuilles de style -->
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'w3.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-common.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-tableau.css')}"/>

				<!-- Script ajoute en parametre -->
				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
					gDefaultAutoReload = <xsl:value-of select="$actualisationClientDefaut"/>;
				</script>
				<!-- Script de navigation par defaut -->
				<script src="{concat($jsPath, 'site-display.js')}"/>

				<title>
					Suivi Compétition - Prochains combats
				</title>
			</head>
			<body>
				<!-- ENTETE -->
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
					<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
					<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
					<xsl:with-param name="affEngagements" select="$affEngagements"/>
					<xsl:with-param name="affStatistiques" select="$affStatistiques"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="selectedItem" select="$selectedItemName"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- CONTENU -->

				<xsl:variable name="titreCompetition">
					<xsl:choose>
						<xsl:when test="$useIntituleCommun = 'true'">
							<xsl:value-of select="SiteConfiguration/@IntituleCommun"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="competition/titre"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>

				<!-- Nom de la competition + Catégorie -->
				<div class="tas-competition-bandeau">
					<div>
						<h4>
							<xsl:value-of select="$titreCompetition"/>
						</h4>
					</div>
					<xsl:if test="$istapis = 'epreuve'">
						<h5>
							<xsl:if test="//epreuve[1]/@sexe='F'">
								Féminines&nbsp;
							</xsl:if>
							<xsl:if test="//epreuve[1]/@sexe='M'">
								Masculins&nbsp;
							</xsl:if>
							<xsl:if test="//epreuve[1]/@sexe='X'">
								Mixte&nbsp;
							</xsl:if>
							<xsl:value-of select="//epreuve[1]/@nom"/>
						</h5>
					</xsl:if>
				</div>

				<xsl:if test="not($msgProchainsCombats = '')">
					<div class="tas-callout tas-callout-warning">
						<button onclick="this.parentElement.style.display='none'" class="tas-callout-close">&times;</button>
						<div>
							<xsl:value-of select="$msgProchainsCombats"/>
						</div>
					</div>
				</xsl:if>

				<!-- Parcours tous les tapis trouves -->
				<div class="w3-padding-small">
					<xsl:for-each select="//tapis">
						<xsl:sort select="@tapis" data-type="number" order="ascending"/>

						<!-- On ne prend en compte que les tapis avec des combats -->
						<xsl:if test="@tapis != 0 and ($istapis != 'epreuve' or count(./combats/combat) &gt; 0)">
							<xsl:variable name="tapis" select="@tapis"/>

							<xsl:call-template name="UnTapis">
								<xsl:with-param name="notapis" select="$tapis"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>
				</div>

				<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
					<script src="{concat($jsPath, 'footer_script.js')}"/>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- TEMPLATE UN TAPIS -->
	<xsl:template name="UnTapis">
		<xsl:param name="notapis"/>
		<xsl:variable name="panelId" select="concat('tapis', $notapis)"/>

		<div class="w3-margin-bottom">
			<button class="ios-accordion-btn" onclick="togglePanel('{$panelId}')">
				<span>
					Tapis <xsl:value-of select="$notapis"/>
				</span>
				<div>
					<img class="tas-accordion-icon tas-icon-hidden" id="{$panelId}Collapse" src="{$imgPath}up_circular-32.png"/>
					<img class="tas-accordion-icon tas-icon-visible" id="{$panelId}Expand" src="{$imgPath}down_circular-32.png"/>
				</div>
			</button>

			<!-- Le contenu du tapis -->
			<div class="tasOpenedPanelType w3-container tas-panel-tableau-combat" id="{$panelId}">
				<!-- La liste des combats -->
				<table class="tas-tableau-prochain-combat w3-margin-top" style="width:100%">
					<tbody>
						<xsl:for-each select="//tapis/combats/combat[ ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]">
							<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>

							<xsl:if test="$istapis = 'epreuve' or position() &lt; number($nbProchainsCombatsEff) or position() = number($nbProchainsCombatsEff)">
								<xsl:call-template name="UnCombat">
									<xsl:with-param name="combat" select="."/>
								</xsl:call-template>
							</xsl:if>
						</xsl:for-each>
					</tbody>
				</table>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE UN COMBAT -->
	<xsl:template name="UnCombat">
		<xsl:param name="combat"/>

		<xsl:variable name="epreuve" select="$combat/@epreuve"/>
		<xsl:variable name="phase" select="$combat/@phase"/>
		<xsl:variable name="typePhase" select="//phase[@id = $phase]/@typePhase"/>

		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="//participant[@judoka = $participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$RefData/structures/clubs/club[@ID = $judoka1/@club]"/>
		<xsl:variable name="comite1" select="$RefData/structures/comites/comite[@ID = $club1/@comite]"/>
		<xsl:variable name="ligue1" select="$RefData/structures/ligues/ligue[@ID = $comite1/@ligue]"/>
		<xsl:variable name="pays1" select="$RefData/structures/lesPays/pays[@ID = $judoka1/@pays]"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="//participant[@judoka = $participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$RefData/structures/clubs/club[@ID = $judoka2/@club]"/>
		<xsl:variable name="comite2" select="$RefData/structures/comites/comite[@ID = $club2/@comite]"/>
		<xsl:variable name="ligue2" select="$RefData/structures/ligues/ligue[@ID = $comite2/@ligue]"/>
		<xsl:variable name="pays2" select="$RefData/structures/lesPays/pays[@ID = $judoka2/@pays]"/>

		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">w3-blue colorized-img-white</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">w3-purple colorized-img-white</xsl:when>
				<xsl:otherwise>w3-lime colorized-img-black</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:if test="count($combat/score[@judoka = 0]) = 0">
			<tr>
				<!-- Judoka 1 -->
				<td style="width:40%; padding-right:4px;">
					<xsl:choose>
						<xsl:when test="$participant1 = 'null'">
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
								<header class="w3-small" style="font-weight: 600;">
									<xsl:value-of select="$judoka1/@nom"/>
									<xsl:if test="$typeCompetition != '1'">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka1/@prenom"/>
									</xsl:if>
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

				<!-- Info Combat -->
				<td class="w3-cell-middle w3-center" style="width:20%">
					<div class="tas-upcoming-info">
						<header class="w3-small" style="font-weight: 600;">
							<xsl:value-of select="//epreuve[@ID = $epreuve]/@sexe"/><xsl:text>&#32;</xsl:text><xsl:value-of select="//epreuve[@ID = $epreuve]/@nom"/>
							(<xsl:call-template name="NiveauTourCombat">
								<xsl:with-param name="combat" select="$combat"/>
								<xsl:with-param name="typePhase" select="$typePhase"/>
								<xsl:with-param name="repechage" select="$combat/feuille/@repechage = 'true'"/>
							</xsl:call-template>)
						</header>
						<footer class="w3-tiny">
							<xsl:if test="$typeCompetition = '1'">
								<div>
									<xsl:attribute name="class">
										tas-prochain-combat-premiere-categorie w3-cell w3-center w3-cell-middle w3-tiny tas-badge-team <xsl:value-of select="$firstrencontreclass"/>
									</xsl:attribute>
									<img class="tas-theme-icon" width="14" style="vertical-align: middle; margin-right: 4px;" src="{$imgPath}starter-32.png"/>
									<xsl:value-of select="$combat/@firstrencontrelib"/>
								</div>
							</xsl:if>
							<xsl:if test="$affDetailCompetition">
								<xsl:value-of select="//epreuve[@ID = $epreuve]/@nom_competition"/>
							</xsl:if>
							<xsl:if test="$affDiscipline">
								<xsl:choose>
									<xsl:when test="//epreuve[@ID = $epreuve]/@discipline_competition = 2">Combat</xsl:when>
									<xsl:when test="//epreuve[@ID = $epreuve]/@discipline_competition = 3">NeWaza</xsl:when>
								</xsl:choose>
								- <xsl:value-of select="//epreuve[@ID = $epreuve]/@nom_cateage"/>
							</xsl:if>
						</footer>
					</div>
				</td>

				<!-- Judoka 2 -->
				<td style="width:40%; padding-left:4px;">
					<xsl:choose>
						<xsl:when test="$participant2 = 'null'">
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
								<header class="w3-small" style="font-weight: 600;">
									<xsl:value-of select="$judoka2/@nom"/>
									<xsl:if test="$typeCompetition != '1'">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka2/@prenom"/>
									</xsl:if>
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
</xsl:stylesheet>