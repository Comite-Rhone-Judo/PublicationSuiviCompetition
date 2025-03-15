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
			<link type="text/css" rel="stylesheet" href="../style/style-classement.css"/>

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
				<xsl:with-param name="selectedItem" select="'classement'"/>
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

			<!-- Le classement -->
			<div class="w3-responsive w3-card w3-small tas-panel-classement">
			<table class="w3-table-all">
				<thead>
					<tr class="w3-light-blue w3-text-indigo">
						<th>&nbsp;</th>
						<th>NOM et Prénom</th>
						<th>Club</th>
						<th>Comité</th>
						<th>Ligue</th>
						<th>Pays</th>
					</tr>
				</thead>
				<tbody>
					<xsl:apply-templates select="//classement/participant">
						<xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
					</xsl:apply-templates>
				</tbody>
			</table>
		</div>

			<div class="w3-container w3-center w3-tiny w3-text-grey tas-footnote">
				<script src="../js/footer_script.js"/>
				<!-- TODO penser a modifier quand on passera en version Participants -->
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Ligne de classement -->
	<xsl:template match="participant">
		<xsl:variable name="participant1" select="@judoka" />
		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="club" select="$j1/@club"/>
		<xsl:variable name="clubN" select="//club[@ID=$club]"/>
		<xsl:variable name="comite" select="$clubN/@comite"/>
		<xsl:variable name="ligue" select="$clubN/@ligue"/>
		<xsl:variable name="position" select="position()"/>

		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="@classementFinal != 0 and @classementFinal &lt; 9">
						<xsl:value-of select="@classementFinal"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>NC</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:value-of select="$j1/@nom"/>
				&nbsp;
				<xsl:value-of select="$j1/@prenom"/>
			</td>
			<td>
				<xsl:value-of select="$clubN/nomCourt"/>
			</td>
			<td>
				<xsl:value-of select="$comite"/>
			</td>
			<td>
				<xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
			</td>
			<td>&nbsp;</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>