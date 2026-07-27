<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/niveau_tour_combat.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/nom_structure.xslt"/>

	<xsl:output method="html" indent="yes" encoding="utf-8"/>

	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="RefData"/>
	<xsl:param name="useIntituleCommun" select="'false'"/>

	<xsl:param name="tailleGroupe"/>
	<xsl:param name="idEcran"/>
	<xsl:param name="tapisAffiches"/>
	<xsl:param name="dispositionAffichage" select="'colonne'"/>
	<xsl:param name="combatsParPageEff"/>
	<xsl:param name="isAffichageCombatLigne" select="'false'"/>
	<xsl:param name="ajusteTexteAuto" select="'false'"/>
	<xsl:param name="afficheCategorieAge" select="'false'"/>
	
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
	<xsl:variable name="typeCompetition" select="/docroot/competition/@type" />
	<xsl:variable name="niveauCompetition" select="/docroot/competition/@niveau" />

	<xsl:variable name="TitreCompetition">
		<xsl:choose>
			<xsl:when test="$useIntituleCommun = 'true'">
				<xsl:value-of select="/docroot/SiteConfiguration/@IntituleCommun"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="/docroot/competition/titre"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>


	<!-- En jujitsu, on affiche la discpline -->
	<xsl:variable select="/docroot/competition/@discipline != 'C_COMPETITION'" name="affDiscipline"/>

	<xsl:variable name="nbProchainsCombatsEff">
		<xsl:choose>
			<xsl:when test="$nbProchainsCombats > 0">
				<xsl:value-of select="$nbProchainsCombats"/>
			</xsl:when>
			<xsl:otherwise>6</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="maxCombatsPage">
		<xsl:choose>
			<xsl:when test="number($combatsParPageEff) > 12">12</xsl:when>
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
			<xsl:when test="$tailleGroupe = '2' and $dispositionAffichage = 'colonne'">50%</xsl:when>
			<xsl:when test="$tailleGroupe = '4'">50%</xsl:when>
			<xsl:when test="$tailleGroupe = '6' and $dispositionAffichage = 'ligne'">50%</xsl:when>
			<xsl:when test="$tailleGroupe = '6' and $dispositionAffichage = 'colonne'">33.333%</xsl:when>
			<xsl:when test="$tailleGroupe = '8' and $dispositionAffichage = 'ligne'">50%</xsl:when>
			<xsl:when test="$tailleGroupe = '8' and $dispositionAffichage = 'colonne'">25%</xsl:when>
			<xsl:otherwise>100%</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="nbLignesTapis">
		<xsl:choose>
			<!-- 2x4 ou 4x2 on ajuste la moitie de la hauteur et quart largeur (Ligne) ou moitie de la largeur et quart de la hauteur (colonne) -->
			<xsl:when test="$tailleGroupe = '8' and $dispositionAffichage = 'ligne'">4</xsl:when>
			<!-- 2x3 ou 3x2 on ajuste la moitie de la hauteur et tiers largeur (Ligne) ou moitie de la largeur et tiers de la hauteur (colonne) -->
			<xsl:when test="$tailleGroupe = '6' and $dispositionAffichage = 'ligne'">3</xsl:when>
			<!-- 1x2 ou 2x1 on ajuste la moitie de la hauteur (Ligne) ou de la largeur (colonne) -->
			<!-- 2x2 on ajuste 1/2 hauteur et largeur -->
			<xsl:when test="$tailleGroupe = '4' 
                     or ($tailleGroupe = '2' and $dispositionAffichage = 'ligne') 
                     or ($tailleGroupe = '6' and $dispositionAffichage = 'colonne') 
                     or ($tailleGroupe = '8' and $dispositionAffichage = 'colonne')">2</xsl:when>
			<!-- 1x1 Ligne ou Colonne, on prend toute la place disponible -->
			<xsl:otherwise>1</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="heightStyle">
		<xsl:choose>
			<xsl:when test="$nbLignesTapis = '1'">calc(100vh - 10.5vh)</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="concat('calc((100vh - 10.5vh) / ', $nbLignesTapis, ')')"/>
			</xsl:otherwise>
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
						<xsl:value-of select="concat($cssPath, 'style-ecran-appel.css?v=', $dateGeneration)"/>
					</xsl:attribute>
				</link>

				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-logger.js')"/>
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
						<br/>
						<span>
							<xsl:choose>
								<xsl:when test="$idEcran = '-1'">(Ecran par défaut)</xsl:when>
								<xsl:otherwise>
									(Ecran n° <xsl:value-of select="$idEcran"/>)
								</xsl:otherwise>
							</xsl:choose>
						</span>
					</div>
				</div>

				<!-- Le contenu de la page -->
				<div id="main-container" class="main-container-flex"
					 data-layout-mode="{$tailleGroupe}"
					 data-duree-rotation="{$delaiDeroulementSec}"
					 data-combats-par-page="{$maxCombatsPage}"
					 data-url-redirecteur="{$urlRedirecteur}"
					 data-auto-ajustement="{$ajusteTexteAuto}">
					<xsl:attribute name="style">
						<xsl:value-of select="concat('--nb-combats: ', $maxCombatsPage, '; --lignes-tapis: ', $nbLignesTapis, ';')"/>
					</xsl:attribute>
					<xsl:for-each select="$tapisAffiches/tapisIds/tapis">
						<xsl:call-template name="UnTapis">
							<xsl:with-param name="notapis" select="@id"/>
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

				<!-- Charge le script d'animation a la fin pour etre sur que le DOM est bien charge -->
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
						<div class="combat-grid-container" id="liste_combats_tapis_{$notapis}">
							<xsl:if test="count($docPrincipal//tapis/combats/combat[ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]) = 0">
								<div class="grid-empty-message w3-center w3-padding-large w3-text-grey w3-xlarge">
									<i>Aucun combat en attente sur ce tapis</i>
								</div>
							</xsl:if>

							<xsl:for-each select="$docPrincipal//tapis/combats/combat[ancestor::tapis/@tapis = $notapis and count(score[@judoka = 0]) = 0]">
								<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>
								<xsl:if test="position() &lt;= number($nbProchainsCombatsEff)">
									<xsl:call-template name="UnCombat">
										<xsl:with-param name="combat" select="."/>
										<xsl:with-param name="indexCombat" select="position()"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:for-each>
						</div>
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
		<xsl:variable name="club1" select="$RefData/structures/clubs/club[@ID = $judoka1/@club]"/>
		<xsl:variable name="comite1" select="$RefData/structures/comites/comite[@ID = $club1/@comite]"/>
		<xsl:variable name="ligue1" select="$RefData/structures/ligues/ligue[@ID = $comite1/@ligue]"/>
		<xsl:variable name="pays1" select="$RefData/structures/lesPays/pays[@ID = $judoka1/@pays]"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="$combat/ancestor::tapis[1]/participants/participant[@judoka = $participant2]/descendant::*[1]"/>
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

		<xsl:variable name="rowClass">
			<xsl:choose>
				<xsl:when test="$indexCombat = 1">grid-combat-row w3-pale-green</xsl:when>
				<xsl:otherwise>grid-combat-row</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<div id="combat_{$combat/@id}" data-row-index="{$indexCombat}" class="{$rowClass}">
			<xsl:attribute name="style">
				<xsl:text>height: </xsl:text>
				<xsl:value-of select="100 div $maxCombatsPage"/>
				<xsl:text>%; max-height: </xsl:text>
				<xsl:value-of select="100 div $maxCombatsPage"/>
				<xsl:text>%;</xsl:text>
			</xsl:attribute>

			<div class="grid-cell-badge">
				<div class="w3-indigo pos-badge">
					<xsl:value-of select="$indexCombat"/>
				</div>
			</div>

			<div class="grid-cell-judoka">
				<div class="judoka-box">
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

					<div class="w3-container">
						<xsl:attribute name="class">
							<xsl:text>w3-container </xsl:text>
							<xsl:choose>
								<xsl:when test="$participant1 = 'null'">
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-attente-ligne</xsl:when>
										<xsl:otherwise>jc-attente-colonne</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-normal-ligne-right</xsl:when>
										<xsl:otherwise>jc-normal-colonne-right</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>

						<xsl:choose>
							<xsl:when test="$participant1 = 'null'">
								<div class="dyn-txt-nom txt-bold">
									<img class="img-attente">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
										</xsl:attribute>
									</img>
									<span class="txt-attente">En Attente</span>
								</div>
							</xsl:when>
							<xsl:otherwise>
								<div>
									<xsl:attribute name="class">
										<xsl:text>dyn-txt-nom </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-2</xsl:when>
											<xsl:otherwise>order-1</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<b>
										<xsl:value-of select="$judoka1/@nom"/>
									</b>
									<xsl:if test="$typeCompetition != '1'">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka1/@prenom"/>
									</xsl:if>
								</div>
								<div>
									<xsl:attribute name="class">
										<xsl:text>dyn-txt-club </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-1 club-ligne-right</xsl:when>
											<xsl:otherwise>order-2 club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>

									<xsl:call-template name="LibelleStructure">
										<xsl:with-param name="ecartement" select="$niveauCompetition" />
										<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
										<xsl:with-param name="club" select="$club1/nomCourt"  />
										<xsl:with-param name="comite" select="$comite1/@ID" />
										<xsl:with-param name="ligue" select="$ligue1/nomCourt"/>
										<xsl:with-param name="pays" select="$pays1/@abr3"/>
									</xsl:call-template>
								</div>
							</xsl:otherwise>
						</xsl:choose>
					</div>
				</div>
			</div>

			<div class="grid-cell-cat">
				<div>
					<xsl:attribute name="class">
						<xsl:text>w3-card w3-pale-yellow w3-round-small cat-box</xsl:text>
						<xsl:if test="$typeCompetition = '1' or $affDiscipline or $afficheCategorieAge = 'true'"> cat-box-equipe</xsl:if>
					</xsl:attribute>
					<div class="dyn-txt-cat-titre">
						<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@sexe"/>
						<xsl:text>&#32;</xsl:text>
						<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@nom"/>
					</div>					
					<div class="dyn-txt-cat-sub">
						(<xsl:call-template name="NiveauTourCombat">
							<xsl:with-param name="combat" select="$combat"/>
							<xsl:with-param name="typePhase" select="$typePhase"/>
							<xsl:with-param name="repechage" select="$combat/feuille/@repechage = 'true'"/>
						</xsl:call-template>)
					</div>
					<xsl:choose>
						<!-- Cas 1 : Compétition Equipe (On garde le vrai cartouche avec couleur) -->
						<xsl:when test="$typeCompetition = '1'">
							<div class="cartouche-equipe-wrapper">
								<div>
									<xsl:attribute name="class">
										<xsl:text>cartouche-equipe w3-tag w3-round-large </xsl:text>
										<xsl:value-of select="substring-before($firstrencontreclass, ' ')"/>
									</xsl:attribute>

									<div>
										<xsl:attribute name="class">
											<xsl:text>cartouche-icone-mask </xsl:text>
											<xsl:value-of select="substring-after($firstrencontreclass, ' ')"/>
										</xsl:attribute>
										<img class="cartouche-icone">
											<xsl:attribute name="src">
												<xsl:value-of select="concat($imgPath, 'starter-32.png')"/>
											</xsl:attribute>
										</img>
									</div>

									<div class="dyn-txt-cat-equipe">
										<xsl:value-of select="$combat/@firstrencontrelib"/>
									</div>
								</div>
							</div>
						</xsl:when>
						<!-- Cas 2 : Option discipline activée (Jujitsu) -->
						<xsl:when test="$affDiscipline">
							<div class="cartouche-equipe-wrapper">
								<!-- L'utilisation de la classe "cartouche-equipe" connecte ce texte au redimensionnement auto JS -->
								<div class="cartouche-equipe w3-text-dark-grey" style="font-weight: bold; opacity: 0.85;">
									<xsl:choose>
										<xsl:when test="$docPrincipal//epreuve[@ID = $epreuve]/@discipline_competition = 2">Combat</xsl:when>
										<xsl:when test="$docPrincipal//epreuve[@ID = $epreuve]/@discipline_competition = 3">NeWaza</xsl:when>
									</xsl:choose>
									<xsl:text> - </xsl:text>
									<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@nom_cateage"/>
								</div>
							</div>
						</xsl:when>
						<!-- Catégorie d'âge seule (Judo classique) -->
						<xsl:when test="$afficheCategorieAge = 'true'">
							<div class="cartouche-equipe-wrapper">
								<div class="cartouche-equipe w3-text-dark-grey" style="font-weight: bold; opacity: 0.85;">
									<xsl:value-of select="$docPrincipal//epreuve[@ID = $epreuve]/@nom_cateage"/>
								</div>
							</div>
						</xsl:when>
					</xsl:choose>
				</div>
			</div>

			<div class="grid-cell-judoka">
				<div class="judoka-box">
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

					<div class="w3-container">
						<xsl:attribute name="class">
							<xsl:text>w3-container </xsl:text>
							<xsl:choose>
								<xsl:when test="$participant2 = 'null'">
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-attente-ligne</xsl:when>
										<xsl:otherwise>jc-attente-colonne</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="$isAffichageCombatLigne = 'true'">jc-normal-ligne-left</xsl:when>
										<xsl:otherwise>jc-normal-colonne-left</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>

						<xsl:choose>
							<xsl:when test="$participant2 = 'null'">
								<div class="dyn-txt-nom txt-bold">
									<img class="img-attente">
										<xsl:attribute name="src">
											<xsl:value-of select="concat($imgPath, 'sablier.png')"/>
										</xsl:attribute>
									</img>
									<span class="txt-attente">En Attente</span>
								</div>
							</xsl:when>
							<xsl:otherwise>
								<div class="dyn-txt-nom order-1">
									<b>
										<xsl:value-of select="$judoka2/@nom"/>
									</b>
									<xsl:if test="$typeCompetition != '1'">
										<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
										<xsl:value-of select="$judoka2/@prenom"/>
									</xsl:if>
								</div>
								<div>
									<xsl:attribute name="class">
										<xsl:text>dyn-txt-club </xsl:text>
										<xsl:choose>
											<xsl:when test="$isAffichageCombatLigne = 'true'">order-2 club-ligne-left</xsl:when>
											<xsl:otherwise>order-2 club-colonne</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									<xsl:call-template name="LibelleStructure">
										<xsl:with-param name="ecartement" select="$niveauCompetition" />
										<xsl:with-param name="typeCompetition" select="$typeCompetition"/>
										<xsl:with-param name="club" select="$club2/nomCourt"  />
										<xsl:with-param name="comite" select="$comite2/@ID" />
										<xsl:with-param name="ligue" select="$ligue2/nomCourt"/>
										<xsl:with-param name="pays" select="$pays2/@abr3"/>
									</xsl:call-template>
								</div>
							</xsl:otherwise>
						</xsl:choose>
					</div>
				</div>
			</div>
		</div>
	</xsl:template>
</xsl:stylesheet>