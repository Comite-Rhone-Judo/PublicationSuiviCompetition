<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="js"></xsl:param>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable select="/competition/@PublierProchainsCombats = 'True'" name="affProchainCombats"/>
	<xsl:variable select="/competition/@PublierAffectationTapis = 'True'" name="affAffectationTapis"/>
	<xsl:variable select="/competition/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/competition/@kinzas" name="affKinzas"/>
	<xsl:variable select="/competition/@Logo" name="logo"/>
	<xsl:variable name="typeCompetition" select="/competition[1]/@type"/>

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
			<link type="text/css" rel="stylesheet" href="../style/style-tableau.css"/>

			<!-- Script de navigation par defaut -->
			<script src="../js/site-display.js"></script>

			<!-- Script ajoute en parametre -->
			<script type="text/javascript">
				<xsl:value-of select="$js"/>
				var delayAutoreloadSec = <xsl:value-of select="$delayActualisationClient"/>;
				window.onload=checkReloading;
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<!-- ENTETE -->
			<xsl:call-template name="entete">
				<xsl:with-param name="logo" select="$logo"/>
				<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
				<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
				<xsl:with-param name="affActualiser" select="'True'"/>
				<xsl:with-param name="selectedItem" select="'avancement'"/>
			</xsl:call-template>

			<!-- CONTENU -->
			<!-- Nom de la competition + Catégorie -->
			<div class="w3-container w3-blue w3-center tas-competition-bandeau">
				<div>
					<h4>
						<xsl:value-of select="./titre"/>
					</h4>
				</div>
				<div class="w3-card w3-indigo">
					<h5>

						<xsl:if test="//epreuve[1]/@sexe='F'">
							Féminines&nbsp;

						</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='M'">
							Masculins&nbsp;

						</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='M'">
							Mixte&nbsp;
						</xsl:if>
						<xsl:value-of select="//epreuve[1]/@nom"/>
					</h5>
				</div>
			</div>

			<!-- Le tableau principal -->
			<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
				<button class="w3-bar-item w3-light-blue" onclick="toggleElement('tableauPrincipal')">
					<img class="img" id="tableauPrincipalCollapse" width="25" src="../img/up_circular-32.png" />
					<img class="img" id="tableauPrincipalExpand" width="25" src="../img/down_circular-32.png" style="display: none;" />
					Tableau principal
				</button>
			</div>
			<div class="w3-container tas-panel-tableau-combat" id="tableauPrincipal">
				<xsl:variable name="repechage">
					<xsl:text>false</xsl:text>
				</xsl:variable>

				<xsl:call-template name="tableau">
					<xsl:with-param name="repechage" select="$repechage"/>
				</xsl:call-template>
			</div>
			<!-- Le tableau repechage s'il existe -->

			<xsl:if test="count(//combat[@repechage = 'true']) &gt; 0">
				<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
					<button class="w3-bar-item w3-light-blue" onclick="toggleElement('tableauRepechages')">
						<img class="img" id="tableauRepechagesCollapse" width="25" src="../img/up_circular-32.png" />
						<img class="img" id="tableauRepechagesExpand" width="25" src="../img/down_circular-32.png" style="display: none;" />
						Tableaux de repêchage
					</button>
				</div>
				<div class="w3-container tas-panel-tableau-combat" id="tableauRepechages">
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
				<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
					<button class="w3-bar-item w3-light-blue" onclick="toggleElement('tableauBarrages')">
						<img class="img" id="tableauBarragesCollapse" width="25" src="../img/up_circular-32.png" />
						<img class="img" id="tableauBarragesExpand" width="25" src="../img/down_circular-32.png" style="display: none;" />
						Tableaux de barrage
					</button>
				</div>
				<div class="w3-container tas-panel-tableau-combat" id="tableauBarrages">
					<xsl:call-template name="tableauBarrage"/>
				</div>
			</xsl:if>

			<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
				<script src="../js/footer_script.js"/>
				<!-- TODO penser a modifier quand on passera en version Participants -->
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- un tableau -->
	<xsl:template name="tableau">
		<xsl:param name="repechage"/>

		<xsl:variable name="prefixRef">
			<xsl:choose>
				<xsl:when test="$repechage='true'">
					<xsl:value-of select="2."/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="1."/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>


		<xsl:variable name="niveau">
			<xsl:for-each
				select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumin">
			<xsl:for-each
				select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="ascending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:variable name="niveaumax">
			<xsl:for-each
				select="//combat[@repechage = $repechage and generate-id() = generate-id(key('combats', @niveau)[1]) and starts-with(@reference, $prefixRef)]">
				<xsl:sort select="@niveau" data-type="number" order="descending"/>
				<xsl:if test="position() = 1">
					<xsl:value-of select="@niveau"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<table>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="$repechage = 'true'">
						tas-tableau-repechage-combat
					</xsl:when>
					<xsl:otherwise>
						tas-tableau-combat
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<tbody>
				<!-- BOUCLE SUR LES COMBATS DU NIVEAU EN COURS -->
				<xsl:for-each select="//combat[@niveau = $niveau and @repechage = $repechage and starts-with(@reference, $prefixRef)]">
					<xsl:sort select="@reference" order="ascending"/>
					<!-- une ligne par combat de 1er niveau -->
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
			<div class="w3-panel w3-small w3-text-blue" style="font-weight: bold;">Barrages 3èmes</div>
			<xsl:call-template name="barrageNiveau">
				<xsl:with-param name="niveau" select="3."/>
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="count(//combat[@repechage = 'true' and starts-with(@reference, '5.')]) &gt; 0">
			<div class="w3-container w3-small w3-text-blue" style="font-weight: bold;">Barrages 5èmes</div>
			<xsl:call-template name="barrageNiveau">
				<xsl:with-param name="niveau" select="5."/>
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="count(//combat[@repechage = 'true' and starts-with(@reference, '7.')]) &gt; 0">
			<div class="w3-container w3-small w3-text-blue" style="font-weight: bold;">Barrages 7èmes</div>
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
					<!-- BOUCLE SUR LES COMBATS -->
					<xsl:for-each select="//combat[@repechage = 'true' and starts-with(@reference, $niveau)]">
						<xsl:sort select="@reference" order="ascending"/>
						<!-- une ligne par combat de 1er niveau -->
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

	<!-- Combat -->
	<!-- TODO Ajouter l'affichage de la categorie qui commence pour une rencontre en equipe -->
	<xsl:template match="combat">
		<!-- DEFINITION DES VARIABLES -->

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

		<!-- hauteur d'une ligne de combat -->
		<xsl:variable name="hcombat">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">
					<xsl:value-of select="-1"/>
				</xsl:when>
				<xsl:when test="$hcombatPrev &gt; -1">
					<xsl:value-of select="35"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="25"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Taille du spacer repechage (sp(0) = 0; sp(n+1) = sp(n) + 35 + f(n) / 2 -->
		<xsl:variable name="spacer">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">
					<xsl:value-of select="-1"/>
				</xsl:when>
				<xsl:when test="$spacerPrev &gt; -1">
					<xsl:value-of select="$spacerPrev + 2 * $hcombatPrev - $hcombat + $fillerPrev div 2"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="0"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Taille du filler repechage (f(0) = 6; f(n+1) = sp(n) + 70 + f(n), max(f(n)/2, 6) si on ne change le nb de combats -->
		<xsl:variable name="filler">
			<xsl:choose>
				<xsl:when test="$repechage = 'false'">
					<xsl:value-of select="-1"/>
				</xsl:when>
				<xsl:when test="$fillerPrev &gt; -1">
					<xsl:choose>
						<!-- Si le nb de combat entre 2 niveaux et le meme, on ne change pas l'espacement -->
						<xsl:when test="$countNiveauPrev = $countNiveau">
							<xsl:choose>
								<xsl:when test="$fillerPrev div 2 &gt; 6">
									<xsl:value-of select="$fillerPrev div 2"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="6"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:when test="not($countNiveauPrev = $countNiveau)">
							<xsl:value-of select="$fillerPrev + 2 * $hcombatPrev + $spacerPrev"/>
						</xsl:when>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="6"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- FORMAT DE COMBAT DU TABLEAU PRINCIPAL -->
		<xsl:if test="$repechage = 'false'">
			<!-- 1ere colonne = niveau 1, 2eme colonne = niveau 2 etc. -->
			<td>
				<xsl:if test="($position - 1) mod $p = 0">
					<xsl:attribute name="rowspan">
						<xsl:value-of select="$p"/>
					</xsl:attribute>
				</xsl:if>

				<!-- une case = 1 combat (div + table de 2 lignes) incluant la barre verticale-->
				<xsl:call-template name="contenuCombat">
					<xsl:with-param name="combat" select="."/>
					<xsl:with-param name="rowspan" select="$p"/>
					<xsl:with-param name="niveaumax" select="$niveaumax"/>
				</xsl:call-template>
			</td>

			<!-- Affichage de la finale -->
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

		<!-- FORMAT DE COMBAT DU TABLEAU DE REPECHAGE -->
		<xsl:if test="$repechage = 'true'">

			<!-- 1ere colonne = niveau 1, 2eme colonne = niveau 2 etc. -->
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

				<!-- une case = 1 combat (div + table de 2 lignes) incluant la barre verticale-->
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

			<!-- Affichage de la finale -->
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

		<!-- RECURSIVITE -->

		<!-- CALCUL DES NOUVELLES VARIABLES -->
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
			<!--<xsl:choose>
        <xsl:when test= "$countNiveauNext = $countNiveau">
          <xsl:value-of select="$position"/>
        </xsl:when>
        <xsl:when test= "$countNiveauNext != $countNiveau">-->
			<xsl:value-of select="(($position - 1) div $p1) + 1"/>
			<!--</xsl:when>
      </xsl:choose>-->
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

	<!-- TODO uniformiser l'affichage pour le 1er tour; mettre l'icone starter en blanc (manque class) -->

	<!-- Le contenu d'un combat du tableau principal -->
	<xsl:template name="contenuCombat">
		<xsl:param name="combat"/>
		<xsl:param name="niveaumax"/>
		<xsl:param name="rowspan"/>

		<!-- Information sur les 2 combattants -->
		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="//participant[@judoka = $participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$judoka1/@club"/>
		<xsl:variable name="comite1" select="//club[@ID = $club1]/@comite"/>
		<xsl:variable name="ligue1" select="//club[@ID = $club1]/@ligue"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="//participant[@judoka = $participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$judoka2/@club"/>
		<xsl:variable name="comite2" select="//club[@ID = $club2]/@comite"/>
		<xsl:variable name="ligue2" select="//club[@ID = $club2]/@ligue"/>

		<!-- Taille dynamique de la div qui n'est pas dans le CSS rowspan * 106px -->
		<xsl:variable name="hdiv" select="106 * $rowspan"/>
		<!-- Taille dynamique d'une 1ere ligne qui n'est pas dans le CSS htr(0) = 25, htrn = 106 * (rowspan -1) / 2 -->
		<xsl:variable name="htrext">
			<xsl:choose>
				<xsl:when test="$rowspan = 1">25</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="(106 * ($rowspan div 2)) div 2"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<!-- Taille dynamique d'une ligne qui n'est pas dans le CSS rowspan htr(0) = 25px, htrn = 106 * (rowspan -1) / 2 - sp / 2 -->
		<xsl:variable name="htrint">
			<xsl:choose>
				<xsl:when test="$rowspan = 1">25</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$htrext - 3"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Extrait la couleur en fonction de la categorie-->
		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">
					w3-blue
				</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">
					w3-purple
				</xsl:when>
				<xsl:otherwise>w3-lime</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- DIV Contenant l'affichage du combat -->
		<!-- une case = 1 combat (div + table de 5 lignes) avec la ligne de jonction -->
		<div class="tas-combat-niveau">
			<xsl:attribute name="style">
				height:<xsl:value-of select="$hdiv"/>px;
			</xsl:attribute>
			<table>
				<!-- Combattant 1-->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrext"/>px;
					</xsl:attribute>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka1/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- 1er niveau est celui des participants -->
											<xsl:when test="$combat/@niveau = $niveaumax">w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-participant</xsl:when>
											<!-- Niveaux suivants ce sont les combats -->
											<xsl:otherwise>w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<!-- Nom du Judoka (complet au debut et en final, uniquement initial dans les combats suivants -->
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut sauf si aucun judoka en face (1er tour) -->
											<xsl:if test="$typeCompetition = 1 and ($combat/@niveau != $niveaumax or $judoka2/@nom)">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>

											<div class="w3-cell">
												<xsl:value-of select="$judoka1/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

												<!-- Sauf en equipe, ajoute la 1ere lettre du prenom avec un . ou le prenom complet au 1er rang -->
												<xsl:if test="$typeCompetition != 1">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of
														select="substring($judoka1/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>
													<!-- 1er niveau, on met le nom complet -->
													<xsl:if test="$combat/@niveau = $niveaumax ">
														<xsl:value-of select="$judoka1/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>

									<!-- Insert le nom du club uniquement au debut du tableau -->
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:variable name="ecartement1" select="//phase[@id = $combat/@phase]/@ecartement"/>
											<xsl:choose>
												<xsl:when test="$ecartement1 = '3'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite1"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite1"/>
													</xsl:if>
												</xsl:when>

												<xsl:when test="$ecartement1 = '4'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of
														select="//ligue[@ID = $ligue1]/nomCourt"/>
													</xsl:if>
												</xsl:when>

												<xsl:otherwise>
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite1"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite1"/>
													</xsl:if>
												</xsl:otherwise>
											</xsl:choose>
										</footer>
									</xsl:if>
								</div>

								<!-- Affiche le score -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>

									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<!-- Combat vide sans score -->
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">
												w3-card w3-container w3-light-grey w3-border w3-right-align tas-participant
											</xsl:when>
											<xsl:otherwise>
												w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut sauf 1er rang -->
											<xsl:if test="$typeCompetition = 1 and $combat/@niveau != $niveaumax">
												<div>
													<xsl:attribute name="class">
														w3-cell tas-combat-premiere-categorie colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
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
								<!-- Affiche le score vide sauf si 1er tour (pas de score en 1er tour) -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>
				<!-- Vertical de groupement -->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrint"/>px;
					</xsl:attribute>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<!-- Vertical de groupement -->
					<td rowspan="3" class="tas-combat-vertical">
						<div class="w3-gray">
							&nbsp;
						</div>
					</td>
				</tr>
				<!-- spacer -->
				<tr class="tas-combat-spacer">
					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td></td>
				</tr>
				<!-- Combattant 2-->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$htrint"/>px;
					</xsl:attribute>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka2/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- 1er niveau est celui des participants -->
											<xsl:when test="$combat/@niveau = $niveaumax">w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-participant</xsl:when>
											<!-- Niveaux suivants ce sont les combats -->
											<xsl:otherwise>w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<!-- Nom du Judoka (complet au debut et en final, uniquement initial dans les combats -->
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut (sauf 1er niveau) -->
											<xsl:if test="$typeCompetition = 1 and ($combat/@niveau != $niveaumax or $judoka1/@nom)">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												<xsl:value-of select="$judoka2/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

												<!-- Sauf en equipe, ajoute la 1ere lettre du prenom avec un . ou le prenom complet au 1er rang -->
												<xsl:if test="$typeCompetition != 1">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of
														select="substring($judoka2/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>

													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka2/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>

									<!-- Insert le nom du club uniquement au debut du tableau -->
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:variable name="ecartement2" select="//phase[@id = $combat/@phase]/@ecartement"/>
											<xsl:choose>
												<xsl:when test="$ecartement2 = '3'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite2"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite2"/>
													</xsl:if>
												</xsl:when>

												<xsl:when test="$ecartement2 = '4'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of
														select="//ligue[@ID = $ligue2]/nomCourt"/>
													</xsl:if>
												</xsl:when>

												<xsl:otherwise>
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite2"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite2"/>
													</xsl:if>
												</xsl:otherwise>
											</xsl:choose>
										</footer>
									</xsl:if>
								</div>

								<!-- Affiche le score -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref2"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>

									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<!-- Combat vide -->
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">
												w3-card w3-container w3-light-grey w3-border w3-right-align tas-participant
											</xsl:when>
											<xsl:otherwise>
												w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut (sauf 1er niveau) -->
											<xsl:if test="$typeCompetition = 1 and $combat/@niveau != $niveaumax">
												<div>
													<xsl:attribute name="class">
														w3-cell tas-combat-premiere-categorie colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												&nbsp;
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">&nbsp;</footer>
									</xsl:if>
								</div>
								<!-- Affiche le score vide -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
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

		<!-- Information sur les 2 combattants -->
		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="//participant[@judoka = $participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$judoka1/@club"/>
		<xsl:variable name="comite1" select="//club[@ID = $club1]/@comite"/>
		<xsl:variable name="ligue1" select="//club[@ID = $club1]/@ligue"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="//participant[@judoka = $participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$judoka2/@club"/>
		<xsl:variable name="comite2" select="//club[@ID = $club2]/@comite"/>
		<xsl:variable name="ligue2" select="//club[@ID = $club2]/@ligue"/>

		<!-- Taille de la barre vertical h(0) = 56; h(n) = 70 + f(n) -->
		<xsl:variable name="hdivbar">
			<xsl:value-of select="$filler + $hcombat + $hcombat"/>
		</xsl:variable>

		<!-- Extrait la couleur en fonction de la categorie-->
		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">
					w3-blue
				</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">
					w3-purple
				</xsl:when>
				<xsl:otherwise>w3-lime</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>


		<!-- DIV de spacer pour decaler les combats -->
		<div>
			<xsl:attribute name="style">
				height:<xsl:value-of select="$spacer"/>px;
			</xsl:attribute>
			&nbsp;
		</div>
		<!-- DIV Contenant l'affichage du combat -->
		<!-- une case = 1 combat (div + table de 5 lignes) avec la ligne de jonction -->
		<div class="tas-combat-repechage-niveau">
			<table>
				<!-- Combattant 1-->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">
							tas-combat-repechage
						</xsl:attribute>
					</xsl:if>
					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka1/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- 1er niveau est celui des participants -->
											<xsl:when test="$combat/@niveau = $niveaumax">w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-participant</xsl:when>
											<!-- Niveaux suivants ce sont les combats -->
											<xsl:otherwise>w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<!-- Nom du Judoka (complet au debut et en final, uniquement initial dans les combats -->
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut -->
											<xsl:if test="$typeCompetition = 1">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">

												<xsl:value-of select="$judoka1/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

												<!-- Sauf en equipe, ajoute la 1ere lettre du prenom avec un . ou le prenom complet au 1er rang -->
												<xsl:if test="$typeCompetition != 1">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of
														select="substring($judoka1/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>

													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka1/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>

									<!-- Insert le nom du club uniquement au debut du tableau -->
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:variable name="ecartement1" select="//phase[@id = $combat/@phase]/@ecartement"/>
											<xsl:choose>
												<xsl:when test="$ecartement1 = '3'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite1"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite1"/>
													</xsl:if>
												</xsl:when>

												<xsl:when test="$ecartement1 = '4'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of
														select="//ligue[@ID = $ligue1]/nomCourt"/>
													</xsl:if>
												</xsl:when>

												<xsl:otherwise>
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite1"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite1"/>
													</xsl:if>
												</xsl:otherwise>
											</xsl:choose>
										</footer>
									</xsl:if>
								</div>

								<!-- Affiche le score -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
									<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>

									<xsl:call-template name="score">
										<xsl:with-param name="combat" select="$combat_prec"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:when>
							<xsl:otherwise>
								<!-- Combat vide sans score -->
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">
												w3-card w3-container w3-light-grey w3-border w3-right-align tas-participant
											</xsl:when>
											<xsl:otherwise>
												w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut -->
											<xsl:if test="$typeCompetition = 1">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">
												&nbsp;
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											&nbsp;
										</footer>
									</xsl:if>
								</div>
								<!-- Affiche le score vide -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
					<td></td>
				</tr>
				<!-- Vertical de groupement -->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">
							tas-combat-repechage
						</xsl:attribute>
					</xsl:if>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<!-- Vertical de groupement -->
					<td rowspan="3" class="tas-combat-repechage-vertical">
						<div class="w3-gray">
							<xsl:attribute name="style">
								height:<xsl:value-of select="$hdivbar"/>px;
							</xsl:attribute>

							&nbsp;
						</div>
					</td>
				</tr>
				<!-- spacer -->
				<tr>
					<xsl:attribute name="style">
						height:<xsl:value-of select="$filler"/>px;
					</xsl:attribute>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td></td>
				</tr>
				<!-- Combattant 2-->
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">
							tas-combat-repechage
						</xsl:attribute>
					</xsl:if>

					<!-- 1ere colonne vide necessaire pour fixer les hauteurs -->
					<td></td>
					<td rowspan="2">
						<xsl:choose>
							<xsl:when test="$judoka2/@nom">
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<!-- 1er niveau est celui des participants -->
											<xsl:when test="$combat/@niveau = $niveaumax">w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-participant</xsl:when>
											<!-- Niveaux suivants ce sont les combats -->
											<xsl:otherwise>w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<!-- Nom du Judoka (complet au debut et en final, uniquement initial dans les combats -->
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut (sauf 1er niveau) -->
											<xsl:if test="$typeCompetition = 1">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>
											<div class="w3-cell">

												<xsl:value-of select="$judoka2/@nom"/>
												<xsl:text disable-output-escaping="yes">&#160;</xsl:text>

												<!-- Sauf en equipe, ajoute la 1ere lettre du prenom avec un . ou le prenom complet au 1er rang -->
												<xsl:if test="$typeCompetition != 1">
													<xsl:if test="$combat/@niveau != $niveaumax">
														<xsl:value-of
														select="substring($judoka2/@prenom, 1, 1)"/>
														<xsl:text disable-output-escaping="yes">.</xsl:text>
													</xsl:if>

													<xsl:if test="$combat/@niveau = $niveaumax">
														<xsl:value-of select="$judoka2/@prenom"/>
													</xsl:if>
												</xsl:if>
											</div>
										</div>
									</header>

									<!-- Insert le nom du club uniquement au debut du tableau -->
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											<xsl:variable name="ecartement2" select="//phase[@id = $combat/@phase]/@ecartement"/>
											<xsl:choose>
												<xsl:when test="$ecartement2 = '3'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite2"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite2"/>
													</xsl:if>
												</xsl:when>

												<xsl:when test="$ecartement2 = '4'">
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of
														select="//ligue[@ID = $ligue2]/nomCourt"/>
													</xsl:if>
												</xsl:when>

												<xsl:otherwise>
													<xsl:if test="$typeCompetition != 1">
														<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
														<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
														<xsl:value-of select="$comite2"/>
													</xsl:if>
													<xsl:if test="$typeCompetition = 1">
														<xsl:value-of select="$comite2"/>
													</xsl:if>
												</xsl:otherwise>
											</xsl:choose>
										</footer>
									</xsl:if>
								</div>

								<!-- Affiche le score -->
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
								<!-- Combat vide -->
								<div>
									<xsl:attribute name="class">
										<xsl:choose>
											<xsl:when test="$combat/@niveau = $niveaumax">
												w3-card w3-container w3-light-grey w3-border w3-right-align tas-participant
											</xsl:when>
											<xsl:otherwise>
												w3-card w3-container w3-pale-yellow w3-border w3-right-align tas-combattant
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<header class="w3-small">
										<div class="w3-cell-row">
											<!-- Pour les competitions en equipes, ajoute la categorie qui commence au debut -->
											<xsl:if test="$typeCompetition = 1">
												<div>
													<xsl:attribute name="class">
														w3-cell colorized-img-white w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align
														<xsl:value-of select="$firstrencontreclass"/>
														<xsl:choose>
															<xsl:when test="$combat/@niveau = $niveaumax">tas-participant-premiere-categorie</xsl:when>
															<xsl:otherwise>
																tas-combat-premiere-categorie
															</xsl:otherwise>
														</xsl:choose>
													</xsl:attribute>
													<img class="img" width="20" src="../img/starter-32.png" />
													<xsl:value-of select="$combat/@firstrencontrelib"/>
												</div>
											</xsl:if>										
											<div class="w3-cell">
												&nbsp;
											</div>
										</div>
									</header>
									<xsl:if test="$combat/@niveau = $niveaumax">
										<footer class="w3-tiny">
											&nbsp;
										</footer>
									</xsl:if>
								</div>
								<!-- Affiche le score vide -->
								<xsl:if test="$combat/@niveau != $niveaumax">
									<xsl:call-template name="scoreVide"/>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
				<tr>
					<xsl:if test ="$combat/@niveau = $niveaumax">
						<xsl:attribute name="class">
							tas-combat-repechage
						</xsl:attribute>
					</xsl:if>

					<td></td>
					<td></td>
				</tr>
			</table>
		</div>
	</xsl:template>

	<!-- Score d'un combat -->
	<xsl:template name="score">
		<xsl:param name="combat"/>

		<xsl:variable name="kinzavainqueur" select="$combat/score[@judoka = $combat/@vainqueur]/@kinza"/>
		<xsl:variable name="kinzaperdant" select="$combat/score[@judoka != $combat/@vainqueur]/@kinza"/>

		<div class="w3-left-align">
			<span class="w3-small">
				<xsl:choose>
					<xsl:when test="$combat/@scorevainqueur != ''">
						<xsl:choose>
							<!-- Individuelle ou Shiai -->
							<xsl:when test="$typeCompetition != '1'">
								<!-- Les marques -->
								<xsl:choose>
									<xsl:when test="$affKinzas = 'Oui'">
										<!-- Les marques avec Kinzas, on ignore le Yuko -->
										<xsl:value-of select="substring($combat/@scorevainqueur, 1, 2)"/>
										<!-- Les kinzas -->
										<span class="w3-small w3-text-green">
											(<xsl:value-of select="$kinzavainqueur"/>)
										</span>
									</xsl:when>
									<!-- Les marques sans Kinzas -->
									<xsl:otherwise>
										<xsl:value-of select="substring($combat/@scorevainqueur, 1, 3)"/>
									</xsl:otherwise>
								</xsl:choose>
								<!-- Les penalites -->
								<span class="w3-text-red">
									<xsl:value-of select="$combat/@penvainqueur"/>
								</span>
							</xsl:when>
							<!-- Equipe -->
							<xsl:otherwise>
								<!-- Uniquement les marques -->
								<xsl:value-of select="$combat/@scorevainqueur"/>
								<!-- Ajoute le V en cas de combat decisif dans la rencontre -->
								<xsl:if test="count($combat/rencontre[@estDecisif='true']) != 0">
									<span class="w3-text-orange"> (V)</span>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:text disable-output-escaping="yes">/</xsl:text>
						<xsl:choose>
							<!-- Individuelle ou Shiai -->
							<xsl:when test="$typeCompetition != '1'">
								<xsl:choose>
									<xsl:when test="$affKinzas = 'Oui'">
										<!-- Les marques avec Kinzas, on ignore le Yuko -->
										<xsl:value-of select="substring($combat/@scoreperdant, 1, 2)"/>
										<!-- Les kinzas -->
										<span class="w3-small w3-text-green">
											(<xsl:value-of select="$kinzaperdant"/>)
										</span>
									</xsl:when>
									<!-- Les marques sans Kinzas -->
									<xsl:otherwise>
										<xsl:value-of select="substring($combat/@scoreperdant, 1, 3)"/>
									</xsl:otherwise>
								</xsl:choose>
								<!-- Les penalites -->
								<span class="w3-text-red">
									<xsl:value-of select="$combat/@penperdant"/>
								</span>
							</xsl:when>
							<!-- Equipe -->
							<xsl:otherwise>
								<!-- Uniquement les marques -->
								<xsl:value-of select="$combat/@scoreperdant"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- Pas de score (combat pas encore realise) -->
					<xsl:otherwise>
						&nbsp;
					</xsl:otherwise>
				</xsl:choose>
			</span>
		</div>
	</xsl:template>

	<!-- Score vide -->
	<xsl:template name="scoreVide">
		<div class="w3-left-align">
			<span class="w3-small">
				&nbsp;
			</span>
		</div>
	</xsl:template>

	<!-- Vainqueur du tableau principal -->
	<xsl:template name="combatVainqueur">
		<xsl:param name="combat"/>
		<xsl:param name="rowspan"/>

		<xsl:variable name="participant1" select="$combat/score[@judoka = $combat/@vainqueur]/@judoka"/>
		<!-- Taille dynamique de la div qui n'est pas dans le CSS rowspan * 100px + 6px -->
		<xsl:variable name="hdivfinal" select="100 * $rowspan + 6"/>

		<div class="tas-combat-final-niveau">
			<xsl:attribute name="style">
				height:<xsl:value-of select="$hdivfinal"/>px;
			</xsl:attribute>

			<table>
				<tbody>
					<tr>
						<td>
							<div class="w3-card w3-container w3-pale-red w3-border w3-right-align tas-combattant">
								<header class="w3-small">
									<xsl:if test="//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom">
										<xsl:value-of select="//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom"/>
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="//participants/participant[@judoka = $participant1]/descendant::*[1]/@prenom"/>
									</xsl:if>
									<xsl:if test="not(//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom)">
										&nbsp;
									</xsl:if>
								</header>
							</div>
							<!-- Affiche le score -->
							<!--
							<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
							<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/> -->

							<xsl:call-template name="score">
								<xsl:with-param name="combat" select="$combat"/>
							</xsl:call-template>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<!-- Vainqueur du tableau de repechage -->
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

		<!-- DIV de spacer pour decaler les combats -->
		<div>
			<xsl:attribute name="style">
				height:<xsl:value-of select="$spacerFinale"/>px;
			</xsl:attribute>
			&nbsp;
		</div>
		<div class="tas-combat-repechage-final-niveau">
			<table>
				<tbody>
					<tr>
						<td>
							<div class="w3-card w3-container w3-pale-red w3-border w3-right-align tas-combattant">
								<header class="w3-small">
									<xsl:if test="//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom">
										<xsl:value-of select="//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom"/>
										<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
										<xsl:value-of select="//participants/participant[@judoka = $participant1]/descendant::*[1]/@prenom"/>
									</xsl:if>
									<xsl:if test="not(//participants/participant[@judoka = $participant1]/descendant::*[1]/@nom)">
										&nbsp;
									</xsl:if>
								</header>
							</div>
							<!-- Affiche le score -->
							<!--<xsl:variable name="ref" select="$combat/feuille/@ref1"/>
							<xsl:variable name="combat_prec" select="//combat[@reference = $ref]"/>-->

							<xsl:call-template name="score">
								<xsl:with-param name="combat" select="$combat"/>
							</xsl:call-template>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<!-- Utilitaire de calcul -->
	<xsl:template name="power">
		<xsl:param name="base"/>
		<xsl:param name="power"/>

		<xsl:variable name="powerTMP">
			<xsl:choose>
				<xsl:when test="power &lt; 0">
					<xsl:value-of select="$power * (-1)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$power"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$power = 0">
				<xsl:value-of select="1"/>
			</xsl:when>
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