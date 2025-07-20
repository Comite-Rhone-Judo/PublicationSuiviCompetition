<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>
	<xsl:import href="Tools/Export/xslt/Site/niveau_tour_combat.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="js"></xsl:param>
	<xsl:param name="istapis"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>


	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable name="couleur1" select="//competition/@couleur1"> </xsl:variable>
	<xsl:variable name="couleur2" select="//competition/@couleur2"> </xsl:variable>
	<xsl:variable name="idCompetition" select="//competition/@ID"> </xsl:variable>
	<xsl:variable name="typeCompetition" select="/competition/@type"> </xsl:variable>

	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable select="/competition/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/competition/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/competition/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/competition/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="number(competition/@NbProchainsCombats)" name="nbProchainsCombats"/>
	<xsl:variable select="/competition/@Logo" name="logo"/>

	<xsl:variable name="nbProchainsCombatsEff">
		<xsl:choose>
			<xsl:when test="$nbProchainsCombats > 0">
				<xsl:value-of select="$nbProchainsCombats"/>
			</xsl:when>
			<xsl:otherwise>6</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<!-- Affiche les details de la competition si on est en judo avec plus d'une competition -->
	<xsl:variable select="count(//epreuve[@competition!=$idCompetition])!=0 and /competition/@disciplineId = 1" name="affDetailCompetition"/>
	
	<!-- En jujitsu, on affiche la discpline -->
	<xsl:variable select="/competition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

	<xsl:variable name="selectedItemName">
		<xsl:choose>
			<xsl:when test="$istapis = 'alltapis'">se_prepare</xsl:when>
			<xsl:otherwise>prochains_combats</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

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
				Suivi Compétition - Avancement
			</title>
		</head>
		<body>
			<!-- ENTETE -->
			<xsl:call-template name="entete">
				<xsl:with-param name="logo" select="$logo"/>
				<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
				<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
				<xsl:with-param name="affEngagements" select="$affEngagements"/>
				<xsl:with-param name="affActualiser" select="true()"/>
				<xsl:with-param name="selectedItem" select="$selectedItemName"/>
				<xsl:with-param name="pathToImg" select="$imgPath"/>
				<xsl:with-param name="pathToCommon" select="$commonPath"/>
			</xsl:call-template>

			<!-- CONTENU -->
			<!-- Nom de la competition + Catégorie si on affiche une epreuve particuliere -->
			<div class="w3-container w3-blue w3-center tas-competition-bandeau">
				<div>
					<h4>
						<xsl:value-of select="./titre"/>
					</h4>
				</div>
				<xsl:if test="$istapis = 'epreuve'">
					<div class="w3-card w3-indigo">
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
					</div>
				</xsl:if>
			</div>

			<xsl:if test="not(/competition/@MsgProchainsCombats = '')">
				<div class="w3-panel w3-khaki w3-display-container w3-card tas-msg-panel w3-cell-row">
					<div class="w3-cell">
						<span onclick="this.parentElement.parentElement.style.display='none'" class="w3-button w3-large w3-display-topright w3-cell-top">&times;</span>
					</div>
					<div class="w3-cell w3-cell-middle">
						<xsl:value-of select="/competition/@MsgProchainsCombats"/>
					</div>
				</div>
			</xsl:if>

			<!-- Parcours tous les tapis trouves -->
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


	<!-- TEMPLATE UN TAPIS -->
	<xsl:template name="UnTapis">
		<xsl:param name="notapis"/>
		<xsl:variable name="apos">'</xsl:variable>


		<!-- Bandeau repliable du tapis -->
		<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
			<button class="w3-bar-item w3-light-blue">
				<xsl:attribute name="onclick">
					<xsl:value-of select="concat('togglePanel(',$apos,'tapis',$notapis,$apos,')')"/>
				</xsl:attribute>
				<img class="img" width="25">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($imgPath, 'up_circular-32.png')"/>
					</xsl:attribute>
					<xsl:attribute name="id">
						<xsl:value-of select="concat('tapis',$notapis,'Collapse')"/>
					</xsl:attribute>
				</img>
				<img class="img" width="25" style="display: none;" >
					<xsl:attribute name="src">
						<xsl:value-of select="concat($imgPath, 'down_circular-32.png')"/>
					</xsl:attribute>
					<xsl:attribute name="id">
						<xsl:value-of select="concat('tapis',$notapis,'Expand')"/>
					</xsl:attribute>
				</img>
				<xsl:value-of select="concat('Tapis&nbsp;',$notapis)"/>
			</button>
		</div>
		<!-- Le contenu du tapis -->
		<div class="tasOpenedPanelType w3-container tas-panel-tableau-combat">
			<xsl:attribute name="id">
				<xsl:value-of select="concat('tapis',$notapis)"/>
			</xsl:attribute>

			<!-- La liste des combats -->
			<table class="w3-table w3-bordered w3-card tas-tableau-prochain-combat" style="width:100%">
				<tbody>
					<!-- Selectionne tous les combats du tapis, sauf ceux "Aucun Judoka", avec les judoka absents -->
					<!-- <xsl:for-each select="//tapis[@tapis = $notapis]/combats/combat"> -->
					<xsl:for-each select="//tapis/combats/combat[ ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]">
						<xsl:sort select="@time_programmation" data-type="number"
							order="ascending"/>

						<!-- Affiche tous les combats de l'epreuve ou les n 1ers du tapis -->
						<xsl:if test="$istapis = 'epreuve' or position() &lt; number($nbProchainsCombatsEff) or position() = number($nbProchainsCombatsEff)">
							<xsl:call-template name="UnCombat">
								<xsl:with-param name="combat" select="."/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>

				</tbody>
			</table>

		</div>
	</xsl:template>

	<!-- TEMPLATE UN COMBAT -->
	<xsl:template name="UnCombat">
		<xsl:param name="combat"/>
		<xsl:param name="img"/>

		<xsl:variable name="epreuve" select="$combat/@epreuve"/>
		<xsl:variable name="phase" select="$combat/@phase"/>
		<xsl:variable name="typePhase" select="//phase[@id = $phase]/@typePhase"/>

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

		<!-- Extrait la couleur en fonction de la categorie-->
		<xsl:variable name="firstrencontreclass">
			<xsl:choose>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'M'">
					w3-blue colorized-img-white
				</xsl:when>
				<xsl:when test="substring($combat/@firstrencontrelib, 1, 1) = 'F'">
					w3-purple colorized-img-white
				</xsl:when>
				<xsl:otherwise>w3-lime colorized-img-black</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Si un des ID judoka vaut zero, c'est une place vide. Si judoka est null, c'est pas encore de combattant, on n'affiche rien -->
		<xsl:if test="count($combat/score[@judoka = 0]) = 0">
		<tr>
			<!-- Judoka 1 -->
			<td style="width:40%">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$participant1 = 'null'">
							<xsl:text disable-output-escaping="yes">w3-sand w3-small w3-card w3-center</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
							<xsl:choose>
								<xsl:when test="$couleur1 = 'Bleu'">
									<xsl:text disable-output-escaping="yes">w3-blue w3-small w3-card w3-right-align</xsl:text>
								</xsl:when>
								<xsl:when test="$couleur1 = 'Rouge'">
									<xsl:text disable-output-escaping="yes">w3-red w3-small w3-card w3-right-align</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text disable-output-escaping="yes">w3-grey w3-small w3-card w3-right-align</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<!-- Affiche le nom du Judoka s'il n'est pas null, "En Attente" sinon -->
				<div class="w3-container">
					<xsl:choose>
						<xsl:when test="$participant1 = 'null'">
							<!-- Combat en attente-->
								<img class="img" width="25">
									<xsl:attribute name="src">
										<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
									</xsl:attribute>
								</img>
							<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<header>
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka = $participant1]/descendant::*[1]/@nom"/>
								<xsl:if test="$typeCompetition != 1">
									<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
									<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka = $participant1]/descendant::*[1]/@prenom"/>
								</xsl:if>
							</header>
							<footer class="w3-tiny">
								<xsl:variable name="ecartement1"
									select="//phase[@id = $phase]/@ecartement"/>
								<xsl:choose>
									<xsl:when test="$ecartement1 = '3'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite1"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite1"/>
										</xsl:if>

									</xsl:when>

									<xsl:when test="$ecartement1 = '4'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
										</xsl:if>
									</xsl:when>

									<xsl:otherwise>
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite1"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite1"/>
										</xsl:if>
									</xsl:otherwise>
								</xsl:choose>
							</footer>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>

			<!-- Info Combat -->
			<td class=" w3-pale-yellow w3-small w3-card w3-cell-middle w3-center"  style="width:20%">
				<!-- Affiche le nom de l'epreuve -->
				<div class="w3-container w3-cell tas-info-combat">
					<header>
						<xsl:value-of select="//epreuve[@ID = $epreuve]/@sexe"/><xsl:text>&#32;</xsl:text><xsl:value-of select="//epreuve[@ID = $epreuve]/@nom"/>
						(<xsl:call-template name="NiveauTourCombat">
							<xsl:with-param name="combat" select="$combat"/>
							<xsl:with-param name="typePhase" select="$typePhase"/>
							<xsl:with-param name="repechage" select="$combat/feuille/@repechage = 'true'"/>
						</xsl:call-template>)
					</header>
					<footer class="w3-tiny">
						<!-- Pour les equipes, affiche la catégorie qui commence -->
						<xsl:if test="$typeCompetition = 1">
							<div>
								<xsl:attribute name="class">tas-prochain-combat-premiere-categorie w3-cell w3-center w3-cell-middle w3-tag w3-round-large w3-tiny w3-left-align <xsl:value-of select="$firstrencontreclass"/></xsl:attribute>
								<img class="img" width="20">
									<xsl:attribute name="src">
										<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
									</xsl:attribute>
								</img>
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
			<td style="width:40%">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$participant2 = 'null'">
							<xsl:text disable-output-escaping="yes">w3-sand w3-small w3-card w3-center</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
							<xsl:choose>
								<xsl:when test="$couleur2 = 'Bleu'">
									<xsl:text disable-output-escaping="yes">w3-blue w3-small w3-card w3-left-align</xsl:text>
								</xsl:when>

								<xsl:when test="$couleur2 = 'Rouge'">
									<xsl:text disable-output-escaping="yes">w3-red w3-small w3-card w3-left-align</xsl:text>
								</xsl:when>

								<xsl:otherwise>
									<xsl:text disable-output-escaping="yes">w3-light-gray w3-small w3-card w3-left-align</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>				
				<!-- Affiche le nom du Judoka s'il n'est pas null, "En Attente" sinon -->
				<div class="w3-container">
					<xsl:choose>
						<xsl:when test="$participant2 = 'null'">
							<!-- Combat en attente-->
								<img class="img" width="25">
									<xsl:attribute name="src">
										<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
									</xsl:attribute>
								</img>
							<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<header>
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka = $participant2]/descendant::*[1]/@nom"/>
								<xsl:if test="$typeCompetition != 1">
									<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
									<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka = $participant2]/descendant::*[1]/@prenom"/>
								</xsl:if>
							</header>
							<footer class="w3-tiny">
								<xsl:variable name="ecartement2"
											select="//phase[@id = $phase]/@ecartement"/>
								<xsl:choose>
									<xsl:when test="$ecartement2 = '3'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite2"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite2"/>
										</xsl:if>
									</xsl:when>

									<xsl:when test="$ecartement2 = '4'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
										</xsl:if>
									</xsl:when>

									<xsl:otherwise>
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite2"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite2"/>
										</xsl:if>
									</xsl:otherwise>
								</xsl:choose>
							</footer>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
		</tr>
		</xsl:if>

	</xsl:template>

</xsl:stylesheet>
