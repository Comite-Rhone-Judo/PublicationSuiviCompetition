<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="js"></xsl:param>


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
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<!-- BANDEAU DE TITRE -->
			<div class="w3-top">
				<div class="w3-cell-row w3-light-grey">
					<button class="w3-cell w3-button w3-xlarge w3-cell-left" onclick="openElement('navigationPanel')">☰</button>
					<div class="w3-cell w3-cell-middle w3-center">
						<h3>Suivi compétition</h3>
					</div>
					<div class="w3-cell w3-cell-middle bandeau-titre">
						<img class="img img-bandeau-titre" src="../img/France-Judo-Rhone.png"/>
					</div>
				</div>
			</div>

			<!-- PANNEAU DE NAVIGATION -->
			<div class="w3-sidebar w3-bar-block w3-border-right w3-animate-left tas-navigation-panel" id="navigationPanel">
				<button onclick="closeElement('navigationPanel')" class="w3-bar-item w3-large">Fermer &times;</button>
				<button class="w3-bar-item w3-button navButton" onclick="location.reload()">Actualiser</button>
				<xsl:if test="$affProchainCombats">
					<a class="w3-bar-item w3-button navButton" href="../common/se-prepare.html">Se prépare</a>
					<a class="w3-bar-item w3-button navButton" href="../common/prochains-combats.html">Prochains combats</a>
				</xsl:if>
				<xsl:if test="$affAffectationTapis">
					<a class="w3-bar-item w3-button navButton" href="../common/affectation_tapis.html">Affectations</a>
				</xsl:if>
				<a class="w3-bar-item w3-button navButton w3-indigo" href="../common/avancement.html">Avancements</a>
				<a class="w3-bar-item w3-button navButton" href="../common/classement.html">Classements</a>
			</div>

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
				<xsl:apply-templates select="//phase/poules/poule"/>
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Les Poules -->
	<xsl:template match="poule">
		<xsl:variable name="numero" select="@numero"/>
		<xsl:variable name="apos">'</xsl:variable>
		<!-- Bandeau repliable de la poule -->
		<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
			<button class="w3-bar-item w3-light-blue">
				<xsl:attribute name="onclick">
					<xsl:value-of select="concat('toggleElement(',$apos,'poule',$numero,$apos,')')"/>
				</xsl:attribute>
				<img class="img" width="25" src="../img/up_circular-32.png">
					<xsl:attribute name="id">
						<xsl:value-of select="concat('poule',$numero,'Collapse')"/>
					</xsl:attribute>
				</img>
				<img class="img" id="poule1Expand" width="25" src="../img/down_circular-32.png" style="display: none;" >
					<xsl:attribute name="id">
						<xsl:value-of select="concat('poule',$numero,'Expand')"/>
					</xsl:attribute>
				</img>
				<xsl:value-of select="concat('Poule&nbsp;',$numero)"/>
			</button>
		</div>
		
		<!-- La poule -->
		  <div class="w3-container w3-animate-top tas-panel-poule-combat">
			  <xsl:attribute name="id">
					<xsl:value-of select="concat('poule',$numero)"/>
				</xsl:attribute>
            <table border="0" class="w3-centered tas-poule-combat">				
				<!-- 1ere ligne entete -->
                <tbody>
                    <tr>
                        <td class="w3-small tas-poule-heading">
                            Combattant
                        </td>
                        <td></td>
						<xsl:for-each select="key('participants', $numero)">
							<td class="tas-poule-heading-combat">
								<div class="w3-center w3-padding-small">
									<span class="w3-badge w3-light-grey">
										<xsl:value-of select="position()"/>
									</span>
								</div>
							</td>
						</xsl:for-each>
                        <td class="w3-small tas-poule-heading">
                            <div class="w3-center w3-padding-small">
                                V
                            </div>
                        </td>
                        <td class="w3-small tas-poule-heading">
                            <div class="w3-center w3-padding-small">
                                Pt
                            </div>
                        </td>
                    </tr>

					<!-- Template par ligne de participant -->
					<xsl:apply-templates select="key('participants', $numero)">
							<xsl:sort select="@position" data-type="number" order="ascending"/>
							<xsl:with-param name="poule" select="$numero"/>
					</xsl:apply-templates>
                </tbody>
            </table>
        </div>
	</xsl:template>
	
	<!-- Les participants -->
	<xsl:template match="participant">
		<xsl:param name="poule"/>

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
			<td class="w3-center w3-border w3-border-black w3-grey tas-poule-combat">
                    &nbsp;
			</td>
			
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
							select="//combat[(score[1][@judoka = $participant1] and score[2][@judoka = $participant2]) or (score[2][@judoka = $participant1] and score[1][@judoka = $participant2])]">
							<xsl:with-param name="participant1" select="$participant1"/>
						</xsl:apply-templates>
					</td>
				</xsl:if>
			</xsl:for-each>	
			
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
				<xsl:attribute name="class">  w3-xxlarge </xsl:attribute>
				<xsl:text>X</xsl:text>
			</xsl:if>
			<xsl:if test="$participant1 = @vainqueur">
				<table class="w3-centered">
					<tr>
						<td class="tas-poule-combat-gagnant">
							<span>
								<xsl:value-of select="substring(./@scorevainqueur, 0, 3)"/>
							</span>
							
							<xsl:if test="$typeCompetition != '1'">
								<span class="w3-text-red">
									<xsl:value-of select="./@penvainqueur"/>
								</span>
							</xsl:if>
						</td>
					</tr>
					<tr>
						<td>
							<span>
								<xsl:value-of select="substring(./@scoreperdant, 0, 3)"/>
							</span>
							<xsl:if test="$typeCompetition != '1'">
								<span class="w3-text-red">
									<xsl:value-of select="./@penperdant"/>
								</span>
							</xsl:if>
						</td>
					</tr>
				</table>
			</xsl:if>
		</div>
	</xsl:template>

</xsl:stylesheet>

