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
	<xsl:param name="idgroupe"></xsl:param>
	<xsl:param name="idcompetition"></xsl:param>

	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
		
	<xsl:variable name="couleur1" select="//competition[@ID = $idcompetition]/@couleur1"> </xsl:variable>
	<xsl:variable name="couleur2" select="//competition[@ID = $idcompetition]/@couleur2"> </xsl:variable>
	<xsl:variable name="typeCompetition" select="//competition[@ID = $idcompetition]/@type"> </xsl:variable>
	
	<xsl:variable select="count(/competitions/competition[@PublierProchainsCombats = 'true']) > 0" name="affProchainCombats"/>
	<xsl:variable select="count(/competitions/competition[@PublierAffectationTapis = 'true']) > 0" name="affAffectationTapis"/>
	<xsl:variable select="count(/competitions/competition[@ParticipantsParEntite = 'true']) > 0" name="affParticipantsParEntite"/>
	<xsl:variable select="count(/competitions/competition[@ParticipantsAbsents = 'true']) > 0" name="affParticipantsAbsents"/>
	<xsl:variable select="sum(/competitions/competition/@DelaiActualisationClientSec) div count(/competitions/competition)" name="delayActualisationClient"/>
	<xsl:variable select="/competitions/competition[1]/@Logo" name="logo"/>

	<xsl:variable name="selectedCompetition" select="/competitions/competition[@ID = $idcompetition]"/>

	<!-- Le groupe selectionne -->
	<xsl:variable select="//groupeParticipants[@id = $idgroupe]" name="selectedGroupeParticipants"/>
	
	<!-- TODO Utile ? -->
	<xsl:variable select="count(//epreuve[@competition!=$idcompetition])!=0" name="affDetailCompetition"/>

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
				<xsl:with-param name="affParticipants" select="true()"/>
				<xsl:with-param name="affActualiser" select="true()"/>
				<xsl:with-param name="selectedItem" select="'participants'"/>
			</xsl:call-template>

			<!-- CONTENU -->
			<xsl:variable name="typeGroupe">
				<xsl:choose>
					<!-- Selection par Entite: le niveau de competition donne le type d'entite -->
					<xsl:when test="$affParticipantsParEntite">
						<xsl:value-of select="./@niveau"/>
					</xsl:when>
					<!-- Selection par Nom: Niveau = 1-->
					<xsl:otherwise>
						<xsl:value-of select="1"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<!-- Nom de la competition + Groupe -->
			<div class="w3-container w3-blue w3-center tas-competition-bandeau">
				<div>
					<h4>
						<xsl:value-of select="./titre"/>
					</h4>
				</div>
				<div class="w3-card w3-indigo">
					<h5>
						<!-- Calcul le titre en fonction du type de groupement affParticipantsParEntite et du niveau de la competition -->
						<xsl:if test="$selectedGroupeParticipants/@sexe = 'F'">
							<xsl:text disable-output-escaping="yes">Féminines,</xsl:text>&nbsp;
						</xsl:if>
						<xsl:if test="$selectedGroupeParticipants/@sexe = 'M'">
							<xsl:text disable-output-escaping="yes">Masculins,</xsl:text>&nbsp;
						</xsl:if>
						<xsl:choose>
							<!-- Niveau Aucun (par Nom) 1 -->
							<xsl:when test="$selectedGroupeParticipants/@type = 1">
								<xsl:text disable-output-escaping="yes">Nom commençant par</xsl:text>&nbsp;<xsl:value-of select="$selectedGroupeParticipants/@entite"/>
							</xsl:when>
							<!-- Niveau Club 2 -->
							<xsl:when test="$selectedGroupeParticipants/@type = 2">
								<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;<xsl:value-of select="//club[@ID = $selectedGroupeParticipants/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau Departement 3 -->
							<xsl:when test="$selectedGroupeParticipants/@type = 3">
								<xsl:text disable-output-escaping="yes">Comité</xsl:text>&nbsp;<xsl:value-of select="//comite[@ID = $selectedGroupeParticipants/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau Ligue 3 -->
							<xsl:when test="$selectedGroupeParticipants/@type = 4">
								<xsl:text disable-output-escaping="yes">Ligue</xsl:text>&nbsp;<xsl:value-of select="//ligue[@ID = $selectedGroupeParticipants/@entite]/nom"/>
							</xsl:when>
							<!-- Niveau National 5 -->
							<!-- Niveau International 6 -->
							<xsl:when test="$selectedGroupeParticipants/@type = 5 or $selectedGroupeParticipants/@type = 6">
								<xsl:value-of select="//pays[@ID = $selectedGroupeParticipants/@entite]/@nom"/>
							</xsl:when>
							<!-- Par defaut, on prend le club -->
							<xsl:otherwise>
								<xsl:text disable-output-escaping="yes">Club</xsl:text>&nbsp;<xsl:value-of select="//club[@ID = $selectedGroupeParticipants/@entite]/nom"/>
							</xsl:otherwise>
						</xsl:choose>
					</h5>
				</div>
			</div>

			<!-- message optionnel -->
			<xsl:if test="not($selectedCompetition/@MsgProchainsCombats = '')">
				<div class="w3-panel w3-khaki w3-display-container w3-card tas-msg-panel w3-cell-row">
					<div class="w3-cell">
						<span onclick="this.parentElement.parentElement.style.display='none'" class="w3-button w3-large w3-display-topright w3-cell-top">&times;</span>
					</div>
					<div class="w3-cell w3-cell-middle">
						<xsl:value-of select="/competition/@MsgProchainsCombats"/>
					</div>
				</div>
			</xsl:if>

			<!-- Selectionne les judokas en fonction du groupement -->
			<!-- Niveau Aucun (par Nom) 1 -->
			<xsl:if test="$selectedGroupeParticipants/@type = 1">
				<xsl:for-each select="$selectedCompetition/judokas/judoka[translate(substring(@nom,1,1), $lowercase, $uppercase)  = translate($selectedGroupeParticipants/@entite, $lowercase, $uppercase)]">
					<xsl:sort select="@nom" order="ascending"/>
					<xsl:call-template name="UnJudoka">
						<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
					</xsl:call-template>
				</xsl:for-each>	
			</xsl:if>
			<!-- Niveau Club 2 -->
			<xsl:if test="$selectedGroupeParticipants/@type = 2">
				<xsl:for-each select="$selectedCompetition/judokas/judoka[@club = $selectedGroupeParticipants/@entite]">
					<xsl:sort select="@nom" order="ascending"/>
					<xsl:call-template name="UnJudoka">
						<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
			<!-- Niveau Departement 3 -->
			<xsl:if test="$selectedGroupeParticipants/@type = 3">
				<xsl:for-each select="$selectedCompetition/judokas/judoka[@comite = $selectedGroupeParticipants/@entite]">
					<xsl:sort select="@nom" order="ascending"/>
					<xsl:call-template name="UnJudoka">
						<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
			<!-- Niveau Ligue 4 -->
			<xsl:if test="$selectedGroupeParticipants/@type = 4">
				<xsl:for-each select="$selectedCompetition/judokas/judoka[@ligue = $selectedGroupeParticipants/@entite]">
					<xsl:sort select="@nom" order="ascending"/>
					<xsl:call-template name="UnJudoka">
						<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
			<!-- Niveau National 5 -->
			<!-- Niveau International 6 -->
			<xsl:if test="$selectedGroupeParticipants/@type = 5 or $selectedGroupeParticipants/@type = 6">
				<xsl:for-each select="$selectedCompetition/judokas/judoka[@pays = $selectedGroupeParticipants/@entite]">
					<xsl:sort select="@nom" order="ascending"/>
					<xsl:call-template name="UnJudoka">
						<xsl:with-param name="niveau" select="$selectedCompetition/@niveau"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
			<!-- Pied de page -->
			<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
				v<xsl:value-of select="$selectedCompetition/@AppVersion"/> - Dernière actualisation: <xsl:value-of select="$selectedCompetition/@DateGeneration"/>
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
		<xsl:if test="@present = 'true' or $affParticipantsAbsents">
			<!-- Bandeau repliable du judoka (ferme par defaut) -->
			<div class="w3-container w3-light-blue w3-text-indigo w3-large w3-bar w3-cell-middle tas-entete-section">
				<button class="w3-bar-item w3-light-blue">
					<xsl:attribute name="onclick">
						<xsl:value-of select="concat('toggleElement(',$apos,'judoka',@id,$apos,')')"/>
					</xsl:attribute>
					<img class="img" width="25" src="../img/up_circular-32.png"  style="display: none;">
						<xsl:attribute name="id">
							<xsl:value-of select="concat('judoka',$idJudoka,'Collapse')"/>
						</xsl:attribute>
					</img>
					<img class="img" width="25" src="../img/down_circular-32.png">
						<xsl:attribute name="id">
							<xsl:value-of select="concat('judoka',$idJudoka,'Expand')"/>
						</xsl:attribute>
					</img>
					<xsl:value-of select="@nom"/>&nbsp;<xsl:value-of select="@prenom"/>
				</button>
			</div>
			<!-- Le contenu du Judoka -->
			<div class="tas-panel-tableau-combat" style="display: none;">
				<xsl:attribute name="id">
					<xsl:value-of select="concat('judoka',$idJudoka)"/>
				</xsl:attribute>
				<xsl:choose>
					<xsl:when test="@present = 'true'">
						<!-- La liste des combats dans lesquel le judo est présent -->
						<xsl:choose>
							<xsl:when test="count($selectedCompetition/combats/combat[score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka]) > 0">
								<xsl:for-each select="$selectedCompetition/epreuves/epreuve">
									<xsl:variable name="idEpreuve" select="@ID"/>
									<!-- On ne prend en compte que les epreuves pour lesquelles il y a des combats -->
									<xsl:if test="count($selectedCompetition/combats/combat[(score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve]) > 0">
											<div>
												<header class="w3-container w3-teal w3-large w3-padding-small">
													<xsl:choose>
														<xsl:when test="@sexe = 'F'">Féminines, </xsl:when>
														<xsl:when test="@sexe = 'M'">Masculins, </xsl:when>
													</xsl:choose>
													<xsl:value-of select="@nom"/>
												</header>
												<div class="w3-container w3-cell w3-cell-middle w3-padding">
													<table class="w3-table w3-bordered tas-tableau-prochain-combat" style="width:100%">
														<tbody>
															<xsl:for-each select="$selectedCompetition/combats/combat[(score[1]/@judoka = $idJudoka or score[2]/@judoka = $idJudoka) and @epreuve = $idEpreuve]">
																<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>
																<xsl:call-template name="UnCombat">
																	<xsl:with-param name="idJudoka" select="$idJudoka"/>
																	<xsl:with-param name="niveau" select="$niveau"/>
																</xsl:call-template>
															</xsl:for-each>
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
		<xsl:param name="idJudoka"></xsl:param>

		<xsl:variable name="epreuve" select="./@epreuve"/>
		<xsl:variable name="phase" select="./@phase"/>

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
			<td style="width:40%">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$judoka1 = 'null'">
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
						<xsl:when test="$judoka1 = 'null'">
							<!-- Combat en attente-->
							<img class="img" src="../img/sablier.png" width="25" />
							<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<header>
								<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka1]/@nom"/>
								<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
								<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka1]/@prenom"/>
							</header>
							<footer class="w3-tiny">
								<!-- On n'affiche par le club/Comite/... du judoka selectionne -->
									<xsl:if test="$judoka1 != $idJudoka">
									<xsl:choose>
										<!-- Niveau Club 2 -->
										<xsl:when test="$niveau = 2">
											<xsl:value-of select="//club[@ID = $club1]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Departement 3 -->										
										<xsl:when test="$niveau = 3">
											<xsl:value-of select="//comite[@ID = $comite1]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Ligue 4 -->
										<xsl:when test="$niveau = 4">
											<xsl:value-of select="//ligue[@ID = $ligue1]/nomCourt"/>
										</xsl:when>
										<!-- Niveau National 5 -->
										<!-- Niveau International 6 -->
										<xsl:when test="$niveau = 5 or $niveau = 6">
											<xsl:value-of select="//pays[@ID = $pays1]/@abr3"/>
										</xsl:when>										
										<!-- Par defaut, on prend le club -->
										<xsl:otherwise>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>
							</footer>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
			<td class=" w3-pale-yellow w3-small w3-card w3-cell-middle w3-center"  style="width:20%">
				<!-- Affiche le Tapis -->
				<div class="w3-container">
						Tapis <xsl:value-of select="./@tapis"/>
				</div>
			</td>
			<td style="width:40%">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$judoka2 = 'null'">
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
						<xsl:when test="$judoka2 = 'null'">
							<!-- Combat en attente-->
							<img class="img" src="../img/sablier.png" width="25" />
							<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<header>
								<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka2]/@nom"/>
								<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
								<xsl:value-of select="ancestor::competition/judokas/judoka[@id = $judoka2]/@prenom"/>
							</header>
							<footer class="w3-tiny">
								<!-- On n'affiche par le club/Comite/... du judoka selectionne -->
								<xsl:if test="not($judoka2 = $idJudoka)">
									<xsl:choose>
										<!-- Niveau Club 2 -->
										<xsl:when test="$niveau = 2">
											<xsl:value-of select="//club[@ID = $club2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Departement 3 -->										
										<xsl:when test="$niveau = 3">
											<xsl:value-of select="//comite[@ID = $comite2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau Ligue 4 -->
										<xsl:when test="$niveau = 4">
											<xsl:value-of select="//ligue[@ID = $ligue2]/nomCourt"/>
										</xsl:when>
										<!-- Niveau National 5 -->
										<!-- Niveau International 6 -->
										<xsl:when test="$niveau = 5 or $niveau = 6">
											<xsl:value-of select="//pays[@ID = $pays2]/@abr3"/>
										</xsl:when>										
										<!-- Par defaut, on prend le club -->
										<xsl:otherwise>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>
							</footer>
							
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
		</tr>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>