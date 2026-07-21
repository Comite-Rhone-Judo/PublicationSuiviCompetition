<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>
	<xsl:param name="RefData"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="/docroot/SiteConfiguration/@kinzas" name="affKinzas"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:variable name="typeCompetition" select="/docroot/competition/@type"/>
	<xsl:variable name="niveauCompetition" select="/docroot/competition/@niveau"/>

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

				<!-- Retour à la balise auto-fermante -->
				<script src="{concat($jsPath, 'site-display.js')}"/>

				<title>Suivi Compétition - Avancement</title>
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
					<xsl:with-param name="selectedItem" select="'avancement'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- Bandeau unifié -->
				<div class="tas-competition-bandeau">
					<h4>
						<xsl:value-of select="competition/titre"/>
					</h4>
					<h5>
						<xsl:if test="//epreuve[1]/@sexe='F'">Féminines&nbsp;</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='M'">Masculins&nbsp;</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='X'">Mixte&nbsp;</xsl:if>
						<xsl:value-of select="//epreuve[1]/@nom"/>
					</h5>
				</div>

				<xsl:choose>
					<!-- Pas de tirage disponible -->
					<xsl:when test="//phase[1]/@etat = 0">
						<div class="w3-padding">
							<div class="ios-card tas-empty-state">Veuillez patienter, le tirage de la phase est en cours...</div>
						</div>
					</xsl:when>
					<!-- Cas standard avec un tirage -->
					<xsl:otherwise>
						<!-- Le tableau principal -->
						<div class="w3-padding-small">
							<button class="ios-accordion-btn" onclick="togglePanel('tableauPrincipal')">
								<span>Tableau principal</span>
								<div>
									<img class="tas-accordion-icon tas-icon-hidden" id="tableauPrincipalCollapse" src="{$imgPath}up_circular-32.png"/>
									<img class="tas-accordion-icon tas-icon-visible" id="tableauPrincipalExpand" src="{$imgPath}down_circular-32.png"/>
								</div>
							</button>
						</div>
						<div class="tasOpenedPanelType w3-container tas-panel-tableau-combat" id="tableauPrincipal">
							<xsl:variable name="repechage">
								<xsl:text>false</xsl:text>
							</xsl:variable>
							<xsl:call-template name="tableau">
								<xsl:with-param name="repechage" select="$repechage"/>
							</xsl:call-template>
						</div>

						<!-- Le tableau repechage s'il existe -->
						<xsl:if test="count(//combat[@repechage = 'true']) &gt; 0">
							<div class="w3-padding-small">
								<button class="ios-accordion-btn" onclick="togglePanel('tableauRepechages')">
									<span>Tableaux de repêchage</span>
									<div>
										<img class="tas-accordion-icon tas-icon-hidden" id="tableauRepechagesCollapse" src="{$imgPath}up_circular-32.png"/>
										<img class="tas-accordion-icon tas-icon-visible" id="tableauRepechagesExpand" src="{$imgPath}down_circular-32.png"/>
									</div>
								</button>
							</div>
							<div class="tasOpenedPanelType w3-container tas-panel-tableau-combat" id="tableauRepechages">
								<xsl:variable name="repechage1">
									<xsl:text>true</xsl:text>
								</xsl:variable>
								<xsl:call-template name="tableau">
									<xsl:with-param name="repechage" select="$repechage1"/>
								</xsl:call-template>
							</div>
						</xsl:if>

						<!-- Les barrages -->
						<xsl:if test="count(//phase[@barrage5 = 'true' or @barrage3 = 'true' or @barrage7 = 'true']) &gt; 0">
							<div class="w3-padding-small">
								<button class="ios-accordion-btn" onclick="togglePanel('tableauBarrages')">
									<span>Tableaux de barrage</span>
									<div>
										<img class="tas-accordion-icon tas-icon-hidden" id="tableauBarragesCollapse" src="{$imgPath}up_circular-32.png"/>
										<img class="tas-accordion-icon tas-icon-visible" id="tableauBarragesExpand" src="{$imgPath}down_circular-32.png"/>
									</div>
								</button>
							</div>
							<div class="tasOpenedPanelType w3-container tas-panel-tableau-combat" id="tableauBarrages">
								<xsl:call-template name="tableauBarrage"/>
							</div>
						</xsl:if>

						<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
							<!-- Retour à la balise auto-fermante -->
							<script src="{concat($jsPath, 'footer_script.js')}"/>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>

	<!-- un tableau -->
	<xsl:template name="tableau">
		<xsl:param name="repechage"/>

		<xsl:variable name="prefixRef">
			<xsl:choose>
				<xsl:when test="$repechage='true'">2.</xsl:when>
				<xsl:otherwise>1.</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="niveau">
			<xsl:for-each select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumin">
			<xsl:for-each select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="ascending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumax">
			<xsl:for-each select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<table>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="$repechage = 'true'">tas-tableau-repechage-combat</xsl:when>
					<xsl:otherwise>tas-tableau-combat	</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<tbody>
				<xsl:for-each select="//combat[@niveau = $niveau and @repechage = $repechage and starts-with(@reference, $prefixRef)]">
					<xsl:sort select="@reference" order="ascending"/>
					<tr>
						<xsl:apply-templates select=".">
							<xsl:with-param name="recursion" select="0"/>
							<xsl:with-param name="position" select="position()"/>
							<xsl:with-param name="repechage" select="$repechage"/>
							<xsl:with-param name="rowspan1" select="0"/>
							<xsl:with-param name="niveauPrev" select="0"/>
							<xsl:with-param name="countNiveauPrev" select="0"/>
							<xsl:with-param name="niveaumax" select="$niveaumax"/>
							<xsl:with-param name="niveaumin" select="$niveaumin"/>
							<xsl:with-param name="fillerPrev" select="-1"/>
							<xsl:with-param name="spacerPrev" select="-1"/>
							<xsl:with-param name="hcombatPrev" select="-1"/>
							<xsl:with-param name="prefixRef" select="$prefixRef"/>
						</xsl:apply-templates>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>

	<!-- Tableaux de barrage -->
	<xsl:template name="tableauBarrage">
		<xsl:if test="count(//combat[@repechage = 'true' and starts-with(@reference, '3.')]) &gt; 0">
			<div class="tas-card-header">Barrages 3èmes</div>
			<xsl:call-template name="barrageNiveau">
				<xsl:with-param name="niveau" select="3."/>
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="count(//combat[@repechage = 'true' and starts-with(@reference, '5.')]) &gt; 0">
			<div class="tas-card-header">Barrages 5èmes</div>
			<xsl:call-template name="barrageNiveau">
				<xsl:with-param name="niveau" select="5."/>
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="count(//combat[@repechage = 'true' and starts-with(@reference, '7.')]) &gt; 0">
			<div class="tas-card-header">Barrages 7èmes</div>
			<xsl:call-template name="barrageNiveau">
				<xsl:with-param name="niveau" select="7."/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<!-- Un niveau (3, 5 ou 7) de tableau de barrage -->
	<xsl:template name="barrageNiveau">
		<xsl:param name="niveau"/>

		<div class="w3-panel">
			<table class="tas-tableau-combat">
				<tbody>
					<xsl:for-each select="//combat[@repechage = 'true' and starts-with(@reference, $niveau)]">
						<xsl:sort select="@reference" order="ascending"/>
						<tr>
							<td>
								<xsl:call-template name="contenuCombat">
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="rowspan" select="1"/>
									<xsl:with-param name="niveaumax" select="0"/>
								</xsl:call-template>
							</td>
							<td>
								<xsl:call-template name="combatVainqueur">
									<xsl:with-param name="combat" select="."/>
									<xsl:with-param name="rowspan" select="1"/>
								</xsl:call-template>
							</td>
						</tr>
					</xsl:for-each>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<!-- Combat principal (calculs préservés) -->
	<xsl:template match="combat">
		<xsl:param name="recursion"/>
		<xsl:param name="position"/>
		<xsl:param name="repechage"/>
		<xsl:param name="rowspan1"/>
		<xsl:param name="niveauPrev"/>
		<xsl:param name="countNiveauPrev"/>
		<xsl:param name="niveaumax"/>
		<xsl:param name="niveaumin"/>
		<xsl:param name="fillerPrev"/>
		<xsl:param name="spacerPrev"/>
		<xsl:param name="hcombatPrev"/>
		<xsl:param name="prefixRef"/>

		<xsl:variable name="p">
			<xsl:call-template name="power">
				<xsl:with-param name="base" select="2"/>
				<xsl:with-param name="power" select="$rowspan1"/>
			</xsl:call-template>
		</xsl:variable>

		<xsl:variable name="niveau" select="@niveau"/>
		<xsl:variable name="countNiveau" select="count(//combat[@niveau = $niveau and @repechage = $repechage and starts-with(@reference, $prefixRef)])"/>

		<xsl:variable name="niveauNext">
			<xsl:for-each select="//combat[@niveau &lt;= $niveau and @repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 2">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="countNiveauNext" select="count(//combat[@niveau = $niveauNext and @repechage = $repechage and starts-with(@reference, $prefixRef)])"/>

		<xsl:variable name="hcombat">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">-1</xsl:when>
				<xsl:when test="$hcombatPrev &gt; -1">35</xsl:when>
				<xsl:otherwise>25</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="spacer">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">-1</xsl:when>
				<xsl:when test="$spacerPrev &gt; -1">
					<xsl:value-of select="$spacerPrev + 2 * $hcombatPrev - $hcombat + $fillerPrev div 2"/>
				</xsl:when>
				<xsl:otherwise>0</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="filler">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">-1</xsl:when>
				<xsl:when test="$fillerPrev &gt; -1">
					<xsl:choose>
						<xsl:when test="$countNiveauPrev = $countNiveau">
							<xsl:choose>
								<xsl:when test="$fillerPrev div 2 &gt; 6">
									<xsl:value-of select="$fillerPrev div 2"/>
								</xsl:when>
								<xsl:otherwise>6</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:when test="not($countNiveauPrev = $countNiveau)">
							<xsl:value-of select="$fillerPrev + 2 * $hcombatPrev + $spacerPrev"/>
						</xsl:when>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>6</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:if test="$repechage = 'false'">
			<td>
				<xsl:if test="($position - 1) mod $p = 0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$p"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:call-template name="contenuCombat">
					<xsl:with-param name="combat" select="."/>
					<xsl:with-param name="rowspan" select="$p"/>
					<xsl:with-param name="niveaumax" select="$niveaumax"/>
				</xsl:call-template>
			</td>
			<xsl:if test="$niveau = $niveaumin">
				<td>
					<xsl:if test="($position - 1) mod $p = 0">
						<xsl:attribute name="rowspan">
							<xsl:value-of select="$p"/>
						</xsl:attribute>
					</xsl:if>
					<xsl:call-template name="combatVainqueur">
						<xsl:with-param name="combat" select="."/>
						<xsl:with-param name="rowspan" select="$p"/>
					</xsl:call-template>
				</td>
			</xsl:if>
		</xsl:if>

		<xsl:if test="$repechage = 'true'">
			<td>
				<xsl:if test="($position - 1) mod $p = 0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$p"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:variable name="affj2">
					<xsl:choose>
						<xsl:when test="$countNiveauPrev = $countNiveau">false</xsl:when>
						<xsl:otherwise>true</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:call-template name="contenuCombatRepechage">
					<xsl:with-param name="combat" select="."/>
					<xsl:with-param name="rowspan" select="$p"/>
					<xsl:with-param name="niveaumax" select="$niveaumax"/>
					<xsl:with-param name="filler" select="$filler"/>
					<xsl:with-param name="spacer" select="$spacer"/>
					<xsl:with-param name="hcombat" select="$hcombat"/>
					<xsl:with-param name="afficheScoreJudoka2" select="$affj2"/>
				</xsl:call-template>
			</td>
			<xsl:if test="$niveau = $niveaumin">
				<td>
					<xsl:if test="($position - 1) mod $p = 0">
						<xsl:attribute name="rowspan">
							<xsl:value-of select="$p"/>
						</xsl:attribute>
					</xsl:if>
					<xsl:call-template name="combatVainqueurRepechage">
						<xsl:with-param name="combat" select="."/>
						<xsl:with-param name="rowspan" select="$p"/>
						<xsl:with-param name="filler" select="$filler"/>
						<xsl:with-param name="spacer" select="$spacer"/>
						<xsl:with-param name="hcombat" select="$hcombat"/>
					</xsl:call-template>
				</td>
			</xsl:if>
		</xsl:if>

		<xsl:variable name="p1">
			<xsl:if test="$countNiveauNext != $countNiveau">
				<xsl:call-template name="power">
					<xsl:with-param name="base" select="2"/>
					<xsl:with-param name="power" select="($recursion + 1)"/>
				</xsl:call-template>
			</xsl:if>
			<xsl:if test="$countNiveauNext = $countNiveau">
				<xsl:value-of select="$p"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="rowspan2">
			<xsl:if test="$countNiveauNext != $countNiveau">
				<xsl:value-of select="$rowspan1 + 1"/>
			</xsl:if>
			<xsl:if test="$countNiveauNext = $countNiveau">
				<xsl:value-of select="$rowspan1"/>
			</xsl:if>
		</xsl:variable>

		<xsl:variable name="p3">
			<xsl:value-of select="(($position - 1) div $p1) + 1"/>
		</xsl:variable>

		<xsl:for-each select="//combat[@niveau = $niveauNext and @repechage = $repechage and starts-with(@reference, $prefixRef)]">
			<xsl:sort select="@reference" order="ascending"/>
			<xsl:if test="position() = $p3">
				<xsl:apply-templates select=".">
					<xsl:with-param name="recursion">
						<xsl:if test="$countNiveauNext != $countNiveau">
							<xsl:value-of select="$recursion + 1"/>
						</xsl:if>
						<xsl:if test="$countNiveauNext = $countNiveau">
							<xsl:value-of select="$recursion"/>
						</xsl:if>
					</xsl:with-param>
					<xsl:with-param name="position" select="$position"/>
					<xsl:with-param name="repechage" select="$repechage"/>
					<xsl:with-param name="rowspan1" select="$rowspan2"/>
					<xsl:with-param name="countNiveauPrev" select="$countNiveau"/>
					<xsl:with-param name="niveauPrev" select="$niveau"/>
					<xsl:with-param name="niveaumax" select="$niveaumax"/>
					<xsl:with-param name="niveaumin" select="$niveaumin"/>
					<xsl:with-param name="fillerPrev" select="$filler"/>
					<xsl:with-param name="spacerPrev" select="$spacer"/>
					<xsl:with-param name="hcombatPrev" select="$hcombat"/>
					<xsl:with-param name="prefixRef" select="$prefixRef"/>
				</xsl:apply-templates>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="contenuCombat">
		<xsl:param name="combat"/>
		<xsl:param name="niveaumax"/>
		<xsl:param name="rowspan"/>

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

		<xsl:variable name="hdiv" select="106 * $rowspan"/>
		<xsl:variable name="htrext">
			<xsl:choose>
				<xsl:when test="$rowspan = 1">25</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="(106 * ($rowspan div 2)) div 2"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="htrint">
			<xsl:choose>
				<xsl:when test="$rowspan = 1">25</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$htrext - 3"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">w3-blue colorized-img-white</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">w3-purple colorized-img-white</xsl:when>
				<xsl:otherwise>w3-lime colorized-img-black</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<div class="tas-combat-niveau">
			<xsl:attribute name="style">
				height:<xsl:value-of select="$hdiv"/>px;
			</xsl:attribute>
			<table>
				<!-- Combattant 1 -->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrext"/>px;
					</xsl:attribute>
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka1/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- Remplacement des vieux w3-pale-yellow par la nouvelle classe sémantique tas-athlete-card -->
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1' and ($combat/@niveau != $niveaumax or $judoka2/@nom)">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												<xsl:value-of select="$judoka1/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
												<xsl:if test="$typeCompetition = '1'">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of select="substring($judoka1/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>
													<xsl:if test="$combat/@niveau = $niveaumax ">
														<xsl:value-of select="$judoka1/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:call-template name="LibelleStructure">
												<xsl:with-param name="ecartement" select="$niveauCompetition" />
												<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
												<xsl:with-param name="club" select="$club1/nomCourt"  />
												<xsl:with-param name="comite" select="$comite1/@ID" />
												<xsl:with-param name="ligue" select="$ligue1/nomCourt"/>
												<xsl:with-param name="pays" select="$pays1/@abr3"/>
											</xsl:call-template>
										</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>
									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- Remplacement des vieux w3-light-grey par tas-athlete-card-empty -->
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card-empty w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1' and $combat/@niveau != $niveaumax">
												<div>
													<xsl:attribute name="class">
														w3-cell tas-combat-premiere-categorie w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">&nbsp;</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">&nbsp;</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>

				<!-- Jonction Verticale (Retrait du w3-gray en dur) -->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrint"/>px;
					</xsl:attribute>
					<td></td>
					<td rowspan="3" class="tas-combat-vertical">
						<div>&nbsp;</div>
					</td>
				</tr>

				<tr class="tas-combat-spacer">
					<td></td>
					<td></td>
				</tr>

				<!-- Combattant 2 -->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrint"/>px;
					</xsl:attribute>
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka2/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1' and ($combat/@niveau != $niveaumax or $judoka1/@nom)">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												<xsl:value-of select="$judoka2/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
												<xsl:if test="$typeCompetition = '1'">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of select="substring($judoka2/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>
													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka2/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:call-template name="LibelleStructure">
												<xsl:with-param name="ecartement" select="$niveauCompetition" />
												<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
												<xsl:with-param name="club" select="$club2/nomCourt"  />
												<xsl:with-param name="comite" select="$comite2/@ID" />
												<xsl:with-param name="ligue" select="$ligue2/nomCourt"/>
												<xsl:with-param name="pays" select="$pays2/@abr3"/>
											</xsl:call-template>
										</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref2"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>
									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card-empty w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1' and $combat/@niveau != $niveaumax">
												<div>
													<xsl:attribute name="class">
														w3-cell tas-combat-premiere-categorie w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">&nbsp;</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">&nbsp;</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrext"/>px;
					</xsl:attribute>
					<td></td>
					<td></td>
				</tr>
			</table>
		</div>
	</xsl:template>

	<!-- Le contenu d'un combat du tableau de repechage -->
	<xsl:template name="contenuCombatRepechage">
		<xsl:param name="combat"/>
		<xsl:param name="niveaumax"/>
		<xsl:param name="rowspan"/>
		<xsl:param name="filler"/>
		<xsl:param name="spacer"/>
		<xsl:param name="hcombat"/>
		<xsl:param name="afficheScoreJudoka2"/>

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

		<xsl:variable name="hdivbar">
			<xsl:value-of select="$filler + $hcombat + $hcombat"/>
		</xsl:variable>

		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">w3-blue colorized-img-white</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">w3-purple colorized-img-white</xsl:when>
				<xsl:otherwise>w3-lime colorized-img-black</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<div>
			<xsl:attribute name="style">
				height:<xsl:value-of select="$spacer"/>px;
			</xsl:attribute>&nbsp;
		</div>

		<div class="tas-combat-repechage-niveau">
			<table>
				<!-- Combattant 1 -->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">tas-combat-repechage</xsl:attribute>
					</xsl:if>
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka1/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1'">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												<xsl:value-of select="$judoka1/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
												<xsl:if test="$typeCompetition != '1'">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of select="substring($judoka1/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>
													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka1/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:call-template name="LibelleStructure">
												<xsl:with-param name="ecartement" select="$niveauCompetition" />
												<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
												<xsl:with-param name="club" select="$club1/nomCourt"  />
												<xsl:with-param name="comite" select="$comite1/@ID" />
												<xsl:with-param name="ligue" select="$ligue1/nomCourt"/>
												<xsl:with-param name="pays" select="$pays1/@abr3"/>
											</xsl:call-template>
										</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>
									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card-empty w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1'">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">&nbsp;</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">&nbsp;</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>

				<!-- Vertical Groupement -->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">tas-combat-repechage</xsl:attribute>
					</xsl:if>
					<td></td>
					<td rowspan="3" class="tas-combat-repechage-vertical">
						<div>
							<xsl:attribute name="style">
								height:<xsl:value-of select="$hdivbar"/>px;
							</xsl:attribute>&nbsp;
						</div>
					</td>
				</tr>

				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$filler"/>px;
					</xsl:attribute>
					<td></td>
					<td></td>
				</tr>

				<!-- Combattant 2 -->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">tas-combat-repechage</xsl:attribute>
					</xsl:if>
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka2/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1'">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												<xsl:value-of select="$judoka2/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
												<xsl:if test="$typeCompetition != '1'">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of select="substring($judoka2/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>
													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka2/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:call-template name="LibelleStructure">
												<xsl:with-param name="ecartement" select="$niveauCompetition" />
												<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
												<xsl:with-param name="club" select="$club2/nomCourt"  />
												<xsl:with-param name="comite" select="$comite2/@ID" />
												<xsl:with-param name="ligue" select="$ligue2/nomCourt"/>
												<xsl:with-param name="pays" select="$pays2/@abr3"/>
											</xsl:call-template>
										</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:choose>
										<xsl:when test="$afficheScoreJudoka2 = 'true'">
											<xsl:variable name="ref" select="$combat/feuille/@ref2"/>
											<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>
											<xsl:call-template name="score">
												<xsl:with-param name="combat" select="$combat_prec"/>
											</xsl:call-template>
										</xsl:when>
										<xsl:otherwise>
											<xsl:call-template name="scoreVide"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">tas-base-card tas-athlete-card-empty w3-right-align tas-participant</xsl:when>
											<xsl:otherwise>tas-base-card tas-athlete-card w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<xsl:if test="$typeCompetition = '1'">
												<div>
													<xsl:attribute name="class">
														w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax"> tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise> tas-combat-premiere-categorie</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20">
														<xsl:attribute name="src">
															<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
														</xsl:attribute>
													</img>
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">&nbsp;</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">&nbsp;</footer>
									</xsl:if>
								</div>
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">tas-combat-repechage</xsl:attribute>
					</xsl:if>
					<td></td>
					<td></td>
				</tr>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="score">
		<xsl:param name="combat"/>

		<xsl:variable name="kinzavainqueur" select="$combat/score[@judoka = $combat/@vainqueur]/@kinza"/>
		<xsl:variable name="kinzaperdant" select="$combat/score[@judoka != $combat/@vainqueur]/@kinza"/>

		<div class="w3-left-align">
			<span class="w3-small">
				<xsl:choose>
					<xsl:when test="$combat/@scorevainqueur != ''">
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:choose>
									<xsl:when test="$affKinzas = 'Oui'">
										<xsl:value-of select="substring($combat/@scorevainqueur, 1, 2)"/>
										<span class="w3-small w3-text-green">
											(<xsl:value-of select="$kinzavainqueur"/>)
										</span>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="substring($combat/@scorevainqueur, 1, 3)"/>
									</xsl:otherwise>
								</xsl:choose>
								<span class="w3-text-red">
									<xsl:value-of select="$combat/@penvainqueur"/>
								</span>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$combat/@scorevainqueur"/>
								<xsl:if test="count($combat/rencontre[@estDecisif='true']) != 0">
									<span class="w3-text-orange"> (V)</span>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:text disable-output-escaping="yes">/</xsl:text>
						<xsl:choose>
							<xsl:when test="$typeCompetition != '1'">
								<xsl:choose>
									<xsl:when test="$affKinzas = 'Oui'">
										<xsl:value-of select="substring($combat/@scoreperdant, 1, 2)"/>
										<span class="w3-small w3-text-green">
											(<xsl:value-of select="$kinzaperdant"/>)
										</span>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="substring($combat/@scoreperdant, 1, 3)"/>
									</xsl:otherwise>
								</xsl:choose>
								<span class="w3-text-red">
									<xsl:value-of select="$combat/@penperdant"/>
								</span>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$combat/@scoreperdant"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>&nbsp;</xsl:otherwise>
				</xsl:choose>
			</span>
		</div>
	</xsl:template>

	<xsl:template name="scoreVide">
		<div class="w3-left-align">
			<span class="w3-small">&nbsp;</span>
		</div>
	</xsl:template>

	<xsl:template name="combatVainqueur">
		<xsl:param name="combat"/>
		<xsl:param name="rowspan"/>

		<xsl:variable name="participant1" select="$combat/score[@judoka = $combat/@vainqueur]/@judoka"/>
		<xsl:variable name="hdivfinal" select="100 * $rowspan + 6"/>

		<div class="tas-combat-final-niveau">
			<xsl:attribute name="style">
				height:<xsl:value-of select="$hdivfinal"/>px;
			</xsl:attribute>
			<table>
				<tbody>
					<tr>
						<td>
							<xsl:variable name="judokaVainqueur" select="//participants/participant[@judoka = $participant1]/descendant::*[1]" />
							<!-- Utilisation de la carte Vainqueur -->
							<div class="tas-base-card tas-athlete-card-winner w3-right-align tas-combattant">
								<header class="w3-small">
									<xsl:if test="$judokaVainqueur/@nom">
										<xsl:value-of select="$judokaVainqueur/@nom"/>
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="$judokaVainqueur/@prenom"/>
									</xsl:if>
									<xsl:if test="not($judokaVainqueur/@nom)">&nbsp;</xsl:if>
								</header>
							</div>
							<xsl:call-template name="score">
								<xsl:with-param name="combat" select="$combat"/>
							</xsl:call-template>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="combatVainqueurRepechage">
		<xsl:param name="combat"/>
		<xsl:param name="rowspan"/>
		<xsl:param name="filler"/>
		<xsl:param name="spacer"/>
		<xsl:param name="hcombat"/>

		<xsl:variable name="participant1" select="$combat/score[@judoka = $combat/@vainqueur]/@judoka"/>
		<xsl:variable name="spacerFinale">
			<xsl:value-of select="$spacer + $hcombat + $filler div 2"/>
		</xsl:variable>

		<div>
			<xsl:attribute name="style">
				height:<xsl:value-of select="$spacerFinale"/>px;
			</xsl:attribute>&nbsp;
		</div>

		<div class="tas-combat-repechage-final-niveau">
			<table>
				<tbody>
					<tr>
						<td>
							<xsl:variable name="judokaVainqueur" select="//participants/participant[@judoka = $participant1]/descendant::*[1]" />
							<!-- Utilisation de la carte Vainqueur -->
							<div class="tas-base-card tas-athlete-card-winner w3-right-align tas-combattant">
								<header class="w3-small">
									<xsl:if test="$judokaVainqueur/@nom">
										<xsl:value-of select="$judokaVainqueur/@nom"/>
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="$judokaVainqueur/@prenom"/>
									</xsl:if>
									<xsl:if test="not($judokaVainqueur/@nom)">&nbsp;</xsl:if>
								</header>
							</div>
							<xsl:call-template name="score">
								<xsl:with-param name="combat" select="$combat"/>
							</xsl:call-template>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="power">
		<xsl:param name="base"/>
		<xsl:param name="power"/>

		<xsl:variable name="powerTMP">
			<xsl:choose>
				<xsl:when test="$power &lt; 0">
					<xsl:value-of select="$power * (-1)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$power"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$power = 0">1</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="temp">
					<xsl:call-template name="power">
						<xsl:with-param name="base" select="$base"/>
						<xsl:with-param name="power" select="$powerTMP - 1"/>
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="$base * $temp"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>