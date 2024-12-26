<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://example.com/2008/data">
	<xsl:import href="Tools/Export/xslt/Site/entete.xslt"/>
	
	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>
	<xsl:template match="/">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:variable select="count(/competitions/competition[@PublierProchainsCombats = 'true']) > 0" name="affProchainCombats"/>
	<xsl:variable select="count(/competitions/competition[@PublierAffectationTapis = 'true']) > 0" name="affAffectationTapis"/>
	<xsl:variable select="count(/competitions/competition[@ParticipantsParEntite = 'true']) > 0" name="affParticipantsParEntite"/>
	<xsl:variable select="/competitions/competition[1]/@Logo" name="logo"/>

	<xsl:template match="/*">
		<xsl:variable name="apos">'</xsl:variable>
		
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

			<!-- Script de navigation par defaut -->
			<script src="../js/site-display.js"/>

			<!-- Script ajoute en parametre -->
			<script type="text/javascript">
				<xsl:value-of select="$js"/>
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
				<xsl:with-param name="affActualiser" select="false()"/>
				<xsl:with-param name="selectedItem" select="'participants'"/>
			</xsl:call-template>

			<!-- CONTENU -->
			<xsl:if test="count(/competitions/competition)=0 or count(//groupeParticipants)=0">
				<div class="w3-container w3-border">
					<div class="w3-panel w3-pale-green w3-bottombar w3-border-green w3-border w3-center w3-large"> Veuillez patienter le tirage des épreuves </div>
				</div>
			</xsl:if>
			
			<!-- Boucle global sur les competitions en cours -->
			<xsl:for-each select="/competitions/competition">
				<xsl:if test="count(./groupesParticipants/groupeParticipants) > 0">
					<xsl:variable name="compet" select="@ID"/>
					<xsl:call-template name="competition">
						<xsl:with-param name="idcompetition" select="$compet"/>
						<xsl:with-param name="parEntite" select="$affParticipantsParEntite"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>

			<xsl:if test="count(/competitions/competition)>0">
				<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
					v<xsl:value-of select="/competition/@AppVersion"/> - Dernière actualisation: <xsl:value-of select="/competitions/competition[1]/@DateGeneration"/>
				</div>
			</xsl:if>

		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Template pour le groupement -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>
		<xsl:param name="parEntite"/>
		
		<xsl:variable name="apos">'</xsl:variable>
		<xsl:variable name="prefixPanel">
			<xsl:value-of select="concat('ParticipantsComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>
		
		<xsl:variable name="typeGroupe">
			<xsl:choose>
				<!-- Selection par Entite: le niveau de competition donne le type d'entite -->
				<xsl:when test="$parEntite">
					<xsl:value-of select="./@niveau"/>
				</xsl:when>
				<!-- Selection par Nom: Niveau = 1-->
				<xsl:otherwise>
					<xsl:value-of select="1"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		

		<!-- Nom de la competition -->
		<div class="w3-container w3-blue w3-center tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Participants" class="w3-container w3-border pane w3-animate-left">
			<!-- une ligne de cellule pour occuper toute le largeur de l'ecran -->
			<div class="w3-cell-row">
				<!-- Chaque panneau est un panel contenant une carte, utilise cell + mobile pour gerer horizontal/vertical selon la taille de l'ecran -->
				<!-- Categorie F -->
				<!-- TODO Modifier pour prendre en compte les participans et pas les epreuves -->
				<xsl:if test="count(./groupesParticipants/groupeParticipants[@sexe = 'F']) > 0">
					<div class="w3-panel w3-cell w3-mobile">
						<!-- La carte des donnees elle meme -->
						<div class="w3-card">
							<!-- entete avec un bouton permet d'ouvrir fermer le contenu de la carte -->
							<header class="w3-bar w3-light-green w3-large">
								<button class="w3-bar-item w3-light-green">
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('toggleElement(',$apos,$prefixPanel,'F',$apos,')')"/>
									</xsl:attribute>
									<img class="img" width="25" src="../img/up_circular-32.png">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'F', 'Collapse')"/>
										</xsl:attribute>
									</img>
									<img class="img" width="25" src="../img/down_circular-32.png" style="display: none;">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'F', 'Expand')"/>
										</xsl:attribute>
									</img>
									Catégorie Féminine
								</button>
							</header>
							<!-- Ajouter les groupes -->
							<div class="w3-row w3-container" style="display:none;">
								<xsl:attribute name="id">
									<xsl:value-of select="concat($prefixPanel,'M')"/>
								</xsl:attribute>
								<xsl:apply-templates select="./groupesParticipants/groupeParticipants[@sexe = 'F' and @type=$typeGroupe]"/>
							</div>
						</div>
					</div>
				</xsl:if>
				<!-- Categorie M -->
				<xsl:if test="count(./groupesParticipants/groupeParticipants[@sexe = 'M']) > 0">
					<div class="w3-panel w3-cell w3-mobile">
						<div class="w3-card">
							<header class="w3-bar w3-light-green w3-large">
								<button class="w3-bar-item w3-light-green">
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('toggleElement(',$apos,$prefixPanel,'M',$apos,')')"/>
									</xsl:attribute>
									<img class="img" width="25" src="../img/up_circular-32.png">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'M', 'Collapse')"/>
										</xsl:attribute>
									</img>
									<img class="img" width="25" src="../img/down_circular-32.png" style="display: none;">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'M', 'Expand')"/>
										</xsl:attribute>
									</img>
									Catégorie Masculine
								</button>
							</header>
							<!-- Ajouter les groupes -->
							<div class="w3-row w3-container" style="display:none;">
								<xsl:attribute name="id">
									<xsl:value-of select="concat($prefixPanel,'M')"/>
								</xsl:attribute>
								<xsl:apply-templates select="./groupesParticipants/groupeParticipants[@sexe = 'M' and @type=$typeGroupe]"/>
							</div>
						</div>
					</div>
				</xsl:if>
				<!-- Sans Categorie -->
				<xsl:if test="count(./groupesParticipants/groupeParticipants[not(@sexe)]) > 0">
					<div class="w3-panel w3-cell w3-mobile">
						<div class="w3-card">
							<header class="w3-bar w3-light-green w3-large">
								<button class="w3-bar-item w3-light-green">
									<xsl:attribute name="onclick">
										<xsl:value-of select="concat('toggleElement(',$apos,$prefixPanel,$apos,')')"/>
									</xsl:attribute>
									<img class="img" width="25" src="../img/up_circular-32.png">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'Collapse')"/>
										</xsl:attribute>
									</img>
									<img class="img" width="25" src="../img/down_circular-32.png" style="display: none;">
										<xsl:attribute name="id">
											<xsl:value-of select="concat($prefixPanel,'Expand')"/>
										</xsl:attribute>
									</img>
									Sans Catégorie
								</button>
							</header>
							<!-- Ajouter les groupes  -->
							<div class="w3-row w3-container" style="display:none;">
								<xsl:attribute name="id">
									<xsl:value-of select="$prefixPanel"/>
								</xsl:attribute>
								<xsl:apply-templates select="./groupesParticipants/groupeParticipants[not(@sexe) and @type=$typeGroupe]"/>
							</div>
						</div>
					</div>
				</xsl:if>
			</div>
		</div>
	</xsl:template>

	<!-- Bouton groupement -->
	<xsl:template name="groupement" match="groupeParticipants">
		<!-- Determine le nom a afficher selon le niveau de la competition -->
		<xsl:variable name="entiteId">
			<xsl:value-of select="./@entite"/>
		</xsl:variable>	
		<xsl:variable name="entiteNom">
			<xsl:choose>
				<!-- Niveau Aucun (par Nom) 1 -->
				<xsl:when test="./@type = 1"><xsl:value-of select="$entiteId"/></xsl:when>
				<!-- Niveau Club 2 -->
				<xsl:when test="./@type = 2"><xsl:value-of select="//club[@ID = $entiteId]/nom"/></xsl:when>
				<!-- Niveau Departement 3 -->
				<xsl:when test="./@type = 3"><xsl:value-of select="//comite[@ID = $entiteId]/nom"/></xsl:when>
				<!-- Niveau Ligue 3 -->
				<xsl:when test="./@type = 4"><xsl:value-of select="//ligue[@ID = $entiteId]/nom"/></xsl:when>
				<!-- Niveau National 5 -->
				<!-- Niveau International 6 -->
				<xsl:when test="./@type = 5 or ./@type = 6"><xsl:value-of select="//pays[@ID = $entiteId]/@nom"/></xsl:when>
				<!-- Par defaut, on prend le club -->
				<xsl:otherwise><xsl:value-of select="//club[@ID = $entiteId]/nom"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>	
		<div>
			<xsl:attribute name="class">
				<xsl:choose>
					<!-- Affiche les noms par Lettre avec un bouton circulaire -->
					<xsl:when test="./@type = '1'">w3-col s3 m2 l1 w3-center w3-padding</xsl:when>
					<xsl:otherwise></xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<a>
				<xsl:attribute name="class">
					<xsl:choose>
						<!-- Affiche les noms par Lettre avec un bouton circulaire -->
						<xsl:when test="./@type = '1'">w3-button w3-card w3-circle w3-xlarge w3-pale-yellow </xsl:when>
						<xsl:otherwise>w3-button w3-panel w3-card w3-block w3-pale-yellow w3-small w3-round-large w3-padding-small</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="href">
					<xsl:text>../participants/</xsl:text>
					<xsl:value-of select="@id"/>
					<xsl:text>/groupe_participants.html</xsl:text>
				</xsl:attribute>
				<!-- Utilise le nom de l'entite retenue en fonction du niveau -->
				<xsl:value-of select="$entiteNom"/>
			</a>
		</div>
	</xsl:template>
</xsl:stylesheet>