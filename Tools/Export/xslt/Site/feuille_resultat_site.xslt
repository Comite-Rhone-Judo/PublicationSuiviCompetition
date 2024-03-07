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
	
	<!-- Type de la poule: 1 = Diagonale, 2 = Colonnes, 3 = auto -->
	<xsl:param name="typePoule"/>
	<xsl:param name="tailleMaxPouleColonne"/>


	<xsl:key name="participants" match="participant" use="@poule"/>
	
	<xsl:variable name="typeCompetition" select="/competition/@type"/>
	
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
			<link type="text/css" rel="stylesheet" href="../style/style-poule.css"/>

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
			
			<!-- Div vide pour aligner le contenu avec le bandeau de titre de taille fixe -->
			<div class="w3-container tas-filler-div">&nbsp;</div>

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
						<xsl:value-of select="//epreuve[1]/@nom"/>						
					</h5>
				</div>
			</div>

			<!-- Les poules -->
			<div class="w3-card">
				<xsl:for-each select="//phase/poules/poule">
					<xsl:variable name="noPoule" ><xsl:value-of select="./@numero"/></xsl:variable>
					<xsl:variable name="phasePoule" select="./@phase"/>
					<xsl:variable name="nbParticipantsPoule" select="count(//participant[@poule = $noPoule and @phase = $phasePoule])"/>

					<!-- Determine la disposition de la poule en fonction du type et de la taille de la poule -->
					<xsl:variable name="dispositionPoule">
						<xsl:choose>
							<!-- Calcul en fonction de la taille de la poule -->
							<xsl:when test="$typePoule = 3">
								<xsl:choose>
									<!-- Diagonal si > max -->
									<xsl:when test="$nbParticipantsPoule > $tailleMaxPouleColonne">1</xsl:when>
									<!-- Colonne si <= max -->
									<xsl:otherwise>2</xsl:otherwise>
								</xsl:choose>
							</xsl:when>
							<!-- Choix force -->
							<xsl:otherwise>
								<xsl:value-of select="$typePoule"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<!-- En fonction de la presence de combats de niveau 2, on affiche ou pas la poule complementaire -->
					<xsl:choose>
						<xsl:when test="count(//combats/combat[@niveau = '2' and @phase = $phasePoule and @reference  = $noPoule]) > 0">
							<xsl:call-template name="templatePoule">
								<xsl:with-param name="niveau" select="1"/>
								<xsl:with-param name="numero" select="$noPoule"/>
								<xsl:with-param name="phase" select="$phasePoule"/>
								<xsl:with-param name="dispositionPoule" select="$dispositionPoule"/>
							</xsl:call-template>
							<xsl:call-template name="templatePoule">
								<xsl:with-param name="niveau" select="2"/> 
								<xsl:with-param name="numero" select="$noPoule"/>
								<xsl:with-param name="phase" select="$phasePoule"/>
								<xsl:with-param name="dispositionPoule" select="$dispositionPoule"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="templatePoule">
								<xsl:with-param name="niveau" select="0"/>
								<xsl:with-param name="numero" select="$noPoule"/>
								<xsl:with-param name="phase" select="$phasePoule"/>
								<xsl:with-param name="dispositionPoule" select="$dispositionPoule"/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
					
				</xsl:for-each>
			</div>

			<!-- Info d'actualisation -->
			<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
				Dernière actualisation: <xsl:value-of select="/competition/@DateGeneration"/>
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Les Poules -->
	<xsl:template name="templatePoule" match="poule">
		<!-- niveau: 0 = poule simple, 1 = poule principale, 2 = Poule complementaire -->
		<xsl:param name="niveau"/>
		<xsl:param name="numero"/>
		<xsl:param name="phase"/>
		<xsl:param name="dispositionPoule"/>
		
		<xsl:variable name="apos">'</xsl:variable>
		<xsl:variable name="niveauCombat">
			<xsl:choose>
				<xsl:when test="$niveau = '2'">2</xsl:when>
				<xsl:otherwise>1</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Bandeau repliable de la poule -->
		<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
			<button class="w3-bar-item w3-light-blue">
				<xsl:attribute name="onclick">
					<xsl:choose>
						<xsl:when test="$niveau > 1">
							<xsl:value-of select="concat('toggleElement(',$apos,'pouleCompl',$numero,$apos,')')"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat('toggleElement(',$apos,'poule',$numero,$apos,')')"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<img class="img" width="25" src="../img/up_circular-32.png">
					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$niveau > 1">
								<xsl:value-of select="concat('pouleCompl',$numero,'Collapse')"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="concat('poule',$numero,'Collapse')"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</img>
				<img class="img" width="25" src="../img/down_circular-32.png" style="display: none;" >
					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$niveau > 1">
								<xsl:value-of select="concat('pouleCompl',$numero,'Expand')"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="concat('poule',$numero,'Expand')"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</img>
				<xsl:choose>
					<xsl:when test="$niveau > 1">
						<xsl:value-of select="concat('Poule&nbsp;',$numero, ' (Complémentaire)')"/>
					</xsl:when>
					<xsl:when test="$niveau = 1">
						<xsl:value-of select="concat('Poule&nbsp;',$numero, ' (Principale)')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat('Poule&nbsp;',$numero)"/>
					</xsl:otherwise>
				</xsl:choose>
			</button>
		</div>
		
		<!-- La poule -->
		  <div class="w3-container tas-panel-poule-combat">
			  <xsl:attribute name="id">
				  <xsl:choose>
					  <xsl:when test="$niveau > 1">
						  <xsl:value-of select="concat('pouleCompl',$numero)"/>
					  </xsl:when>
					  <xsl:otherwise>
						  <xsl:value-of select="concat('poule',$numero)"/>
					  </xsl:otherwise>
				  </xsl:choose>
			  </xsl:attribute>
            <table border="0" class="w3-centered tas-poule-combat">				
				<!-- 1ere ligne entete -->
                <tbody>
                    <tr>
                        <td class="w3-small tas-poule-heading">
                            Combattant
                        </td>
                        <td></td>
						<xsl:choose>
							<xsl:when test="$dispositionPoule = 2">
								<!-- Disposition en colonne, les entetes sont les combats -->
								<xsl:for-each select="//combat[ @niveau = $niveauCombat and @phase = $phase and @reference = $numero]">
									<xsl:sort select="@numero" order="ascending"/>

									<!-- Calcul les positions des judokas -->
									<xsl:variable name="posj1">
										<xsl:call-template name="positionJudoka">
											<xsl:with-param name="noPoule" select="$numero"/>
											<xsl:with-param name="idJudoka" select="./score[1]/@judoka"/>
										</xsl:call-template>
									</xsl:variable>

									<xsl:variable name="posj2">
										<xsl:call-template name="positionJudoka">
											<xsl:with-param name="noPoule" select="$numero"/>
											<xsl:with-param name="idJudoka" select="./score[2]/@judoka"/>
										</xsl:call-template>
									</xsl:variable>

									<!-- Affiche les positions judokas du combat en entete -->
									<td class="tas-poule-heading-combat">
										<div class="w3-center w3-padding-small">
											<span class="w3-tag w3-round-large w3-light-grey">
												<xsl:value-of select="$posj1"/> - <xsl:value-of select="$posj2"/>
											</span>
										</div>
									</td>
								</xsl:for-each>
							</xsl:when>
							<xsl:otherwise>
								<!-- Disposition en diagonale, les entetes sont les participants -->
								<xsl:for-each select="key('participants', $numero)">
									<td class="tas-poule-heading-combat">
										<div class="w3-center w3-padding-small">
											<span class="w3-badge w3-light-grey">
												<xsl:value-of select="position()"/>
											</span>
										</div>
									</td>
								</xsl:for-each>
							</xsl:otherwise>
						</xsl:choose>
                        <td class="w3-small tas-poule-heading">
                            <div class="w3-center w3-padding-small">
                                V
                            </div>
                        </td>
                        <td class="w3-small tas-poule-heading">
                            <div class="w3-center w3-padding-small">

								<xsl:choose>
									<xsl:when test="$typeCompetition != '1'">Pt</xsl:when>
									<xsl:when test="$typeCompetition = '1'">Score</xsl:when>
								</xsl:choose>
                            </div>
                        </td>
                    </tr>

					<!-- Template par ligne de participant -->
					<xsl:apply-templates select="key('participants', $numero)">
							<xsl:sort select="@position" data-type="number" order="ascending"/>
							<xsl:with-param name="poule" select="$numero"/>
							<xsl:with-param name="phase" select="$phase"/>
							<xsl:with-param name="dispositionPoule" select="$dispositionPoule"/>
						<xsl:with-param name="niveauCombat" select="$niveauCombat"/>
					</xsl:apply-templates>
                </tbody>
            </table>
        </div>
	</xsl:template>
	
	<!-- Les participants -->
	<xsl:template match="participant">
		<xsl:param name="poule"/>
		<xsl:param name="phase"/>
		<xsl:param name="niveauCombat"/>
		<xsl:param name="dispositionPoule"/>

		<xsl:variable name="participant1" select="@judoka"/>
		<xsl:variable name="j1"
			select="//participants/participant[@judoka = $participant1]/descendant::*[1]"/>
		<xsl:variable name="grade" select="$j1/@grade"/>
		<tr>
			<td>
                <div class="w3-card w3-container w3-pale-yellow w3-border w3-right-align">
                    <header class="w3-small">
						<xsl:value-of select="$j1/@nom"/>
						<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
						<xsl:value-of select="$j1/@prenom"/>
					</header>					 
                    <footer class="w3-tiny">
						<xsl:variable name="club" select="$j1/@club"/>
						<xsl:variable name="clubN" select="//club[@ID = $club]"/>
						<xsl:variable name="comite" select="$clubN/@comite"/>
						<xsl:variable name="ligue" select="$clubN/@ligue"/>


						<xsl:variable name="ecartement" select="ancestor::phase[1]/@ecartement"/>
						<xsl:choose>
							<xsl:when test="$ecartement = '3'">
								<xsl:value-of select="$clubN/nomCourt"/> - <xsl:value-of select="$comite"/>
							</xsl:when>
										
							<xsl:when test="$ecartement = '4'">
								<xsl:value-of select="$clubN/nomCourt"/> - <xsl:value-of select="//ligue[@ID = $ligue]/nomCourt"/>
							</xsl:when>

							<xsl:otherwise>
								<xsl:value-of select="$clubN/nomCourt"/>
							</xsl:otherwise>
						</xsl:choose>
					</footer>
                </div>
            </td>
			<td class="w3-center w3-padding-small">
                <div class="w3-badge w3-light-grey">
					<xsl:value-of select="position()"/>
				</div>
            </td>
			<!-- Les combats de la ligne du participant -->
			<xsl:choose>
				<!-- Disposition en colonnes -->
				<xsl:when test="$dispositionPoule = 2">
					<xsl:for-each select="//combat[ @niveau = $niveauCombat and @phase = $phase and @reference = $poule]">
						<xsl:sort select="numero" order="ascending"/>

						<xsl:variable name="combatj1" select="./score[1]/@judoka"/>
						<xsl:variable name="combatj2" select="./score[2]/@judoka"/>

						<!-- Case ne correspondant pas au combat -->
						<xsl:if test="$participant1 != $combatj1 and $participant1 != $combatj2">
							<td class="w3-center w3-border w3-border-black w3-grey tas-poule-combat">
								&nbsp;
							</td>
						</xsl:if>
						
						<!-- Case correspondant a un des judokas du combat -->
						<xsl:if test="$participant1 = $combatj1 or $participant1 = $combatj2">
							<td class="w3-center w3-border w3-border-black tas-poule-combat">
								<xsl:apply-templates select=".">
									<xsl:with-param name="participant1" select="$participant1"/>
								</xsl:apply-templates>
							</td>
						</xsl:if>						
					</xsl:for-each>
				</xsl:when>
				<!-- Disposition en diagonale -->
				<xsl:otherwise>
					<xsl:for-each select="key('participants', $poule)">
						<xsl:sort select="@position" data-type="number" order="ascending"/>
						<xsl:variable name="participant2" select="@judoka"/>

						<xsl:if test="$participant1 = $participant2">
							<td class="w3-center w3-border w3-border-black w3-grey tas-poule-combat">
								&nbsp;
							</td>
						</xsl:if>
						<xsl:if test="$participant1 != $participant2">
							<td class="w3-center w3-border w3-border-black tas-poule-combat">
								<xsl:apply-templates
									select="//combat[ @niveau = $niveauCombat and ((score[1][@judoka = $participant1] and score[2][@judoka = $participant2]) or (score[2][@judoka = $participant1] and score[1][@judoka = $participant2]))]">
									<xsl:with-param name="participant1" select="$participant1"/>
								</xsl:apply-templates>
							</td>
						</xsl:if>
					</xsl:for-each>
				</xsl:otherwise>
			</xsl:choose>			
			<td class="w3-panel">
                    <span class="w3-small">
						<xsl:value-of select="@nbVictoires"/>					 
					</span>
                </td>
			<td class="w3-panel">
                    <span class="w3-small">
						<xsl:value-of select="@cumulPoints"/>
					</span>
                </td>
		</tr>
	</xsl:template>
	
	<!-- Combat sans resultat -->
	<xsl:template match="combat[not(@vainqueur) or @vainqueur = '-1']">
		<div class="w3-padding-small w3-xxlarge">
            &nbsp;
        </div>
	</xsl:template>	
	
	<!-- Combat avec resultat -->
	<xsl:template match="combat[@vainqueur and @vainqueur != '-1']">
		<xsl:param name="participant1"/>
		
		<xsl:variable name="kinzavainqueur" select="./score[@judoka = ancestor::combat/@vainqueur]/@kinza"/>
		<xsl:variable name="kinzaperdant" select="./score[@judoka != ancestor::combat/@vainqueur]/@kinza"/>

		<xsl:variable name="participant2">
			<xsl:if test="$participant1 != ./score[1]/@judoka">
				<xsl:value-of select="./score[1]/@judoka"/>		
			</xsl:if>
			<xsl:if test="$participant1 != ./score[2]/@judoka">
				<xsl:value-of select="./score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>
		
		<div class="w3-padding-small">
			<xsl:if test="$participant1 != @vainqueur">
				<xsl:attribute name="class">w3-xxxlarge </xsl:attribute>
				<xsl:text>X</xsl:text>
			</xsl:if>
			<xsl:if test="$participant1 = @vainqueur">
				<table class="w3-centered">
					<tr>
						<td class="tas-poule-combat-gagnant">
							<span>
								<xsl:choose>
									<xsl:when test="$typeCompetition != '1'">
										<xsl:value-of select="substring(./@scorevainqueur, 0, 3)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="./@scorevainqueur"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							
							<xsl:if test="$typeCompetition != '1'">
								<span class="w3-text-red">
									<xsl:value-of select="./@penvainqueur"/>
								</span>
								<xsl:if test="$affKinzas = 'Oui'">
									<span class="w3-small w3-text-green">
										(<xsl:value-of select="$kinzavainqueur"/>)
									</span>
								</xsl:if>
							</xsl:if>
						</td>
					</tr>
					<tr>
						<td>
							<span>
								<xsl:choose>
									<xsl:when test="$typeCompetition != '1'">
										<xsl:value-of select="substring(./@scoreperdant, 0, 3)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="./@scoreperdant"/>
									</xsl:otherwise>
								</xsl:choose>
								
							</span>
							<xsl:if test="$typeCompetition != '1'">
								<span class="w3-text-red">
									<xsl:value-of select="./@penperdant"/>
								</span>
								<xsl:if test="$affKinzas = 'Oui'">
									<span class="w3-small w3-text-green">
										(<xsl:value-of select="$kinzaperdant"/>)
									</span>
								</xsl:if>
							</xsl:if>
						</td>
					</tr>
				</table>
			</xsl:if>
		</div>
	</xsl:template>

	<!-- Calcul de  la position d'un judoka dans la liste des participants -->
	<xsl:template name="positionJudoka">
		<xsl:param name="noPoule"/>
		<xsl:param name="idJudoka"/>

		<xsl:variable name="tempVar">
			<xsl:for-each select="key('participants', $noPoule)">
				<xsl:sort select="@position" data-type="number" order="ascending"/>
				<xsl:if test="./@ID = $idJudoka">
					<xsl:value-of select="position()"/>
				</xsl:if>
			</xsl:for-each>
		</xsl:variable>

		<xsl:choose>
			<xsl:when test="$tempVar">
				<xsl:value-of select="$tempVar"/>
			</xsl:when>
			<xsl:otherwise>-1</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>