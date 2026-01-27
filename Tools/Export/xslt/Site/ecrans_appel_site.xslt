<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html" indent="yes" encoding="utf-8"/>

	<xsl:param name="imgPath" select="'img/site/'"/>
	<xsl:param name="jsPath" select="'js/'"/>
	<xsl:param name="cssPath" select="'style/site/'"/>

	<xsl:param name="LayoutMode" select="'4'"/>
	<xsl:param name="DureeRotation" select="'15'"/>
	<xsl:param name="NbCombatsParPage" select="'6'"/>
	<xsl:param name="TitreCompetition" select="/Competition/@Nom"/>

	<xsl:variable name="widthStyle">
		<xsl:choose>
			<xsl:when test="$LayoutMode = '1'">100%</xsl:when>
			<xsl:when test="$LayoutMode = '2'">50%</xsl:when>
			<xsl:when test="$LayoutMode = '4'">50%</xsl:when>
			<xsl:otherwise>50%</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="heightStyle">
		<xsl:choose>
			<xsl:when test="$LayoutMode = '1'">100%</xsl:when>
			<xsl:when test="$LayoutMode = '2'">100%</xsl:when>
			<xsl:when test="$LayoutMode = '4'">50%</xsl:when>
			<xsl:otherwise>50%</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:template match="/">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<title>
					Ecran Appel - <xsl:value-of select="$TitreCompetition"/>
				</title>
				<meta name="viewport" content="width=device-width, initial-scale=1"/>

				<link rel="stylesheet" type="text/css">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'w3.css')"/>
					</xsl:attribute>
				</link>
				<link rel="stylesheet" type="text/css">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'style-common.css')"/>
					</xsl:attribute>
				</link>
				<link rel="stylesheet" type="text/css">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'style-ecransappel.css')"/>
					</xsl:attribute>
				</link>
			</head>

			<body class="w3-black w3-sans-serif">

				<div class="tv-header w3-white w3-card">
					<img alt="Logo" class="tv-logo" onerror="this.style.display='none'">
						<xsl:attribute name="src">
							<xsl:value-of select="concat($imgPath, 'logo-France-Judo.png')"/>
						</xsl:attribute>
					</img>
					<div class="tv-title w3-text-black">
						<xsl:value-of select="$TitreCompetition"/>
					</div>
				</div>

				<div id="main-container"
					 data-layout-mode="{$LayoutMode}"
					 data-duree-rotation="{$DureeRotation}"
					 data-combats-par-page="{$NbCombatsParPage}">

					<xsl:apply-templates select="//Tapis">
						<xsl:sort select="@Numero" data-type="number"/>
					</xsl:apply-templates>

				</div>

				<div id="progress-container" class="w3-dark-grey">
					<div id="progress-bar" class="w3-blue"></div>
				</div>

				<script type="text/javascript">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'animation.js')"/>
					</xsl:attribute>
				</script>

			</body>
		</html>
	</xsl:template>


	<xsl:template match="Tapis">

		<xsl:variable name="Position" select="position()"/>
		<xsl:variable name="pageIndex" select="floor(($Position - 1) div $LayoutMode) + 1" />

		<div id="tapis_{@ID}"
			 class="tapis-card w3-animate-opacity"
			 data-tapis-page="{$pageIndex}"
			 data-tapis-numero="{@Numero}"
			 style="display:none; width: {$widthStyle}; height: {$heightStyle};">

			<div class="tapis-inner w3-white w3-round-large w3-card-4">

				<div class="tapis-header w3-red w3-center w3-display-container">
					<span class="w3-display-topleft w3-padding w3-medium w3-opacity">
						P<xsl:value-of select="$pageIndex"/>
					</span>
					<span class="w3-display-topright w3-padding w3-large" id="paging_indicator_tapis_{@ID}">1/1</span>
					<b>
						Tapis <xsl:value-of select="@Numero"/>
					</b>
				</div>

				<div class="tapis-content">
					<table class="combat-list w3-table w3-striped">
						<thead>
							<tr class="w3-dark-grey">
								<th class="w3-center" style="width:8%">#</th>
								<th class="w3-center" style="width:12%">Cbt</th>
								<th style="width:35%">Blanc</th>
								<th style="width:35%">Bleu</th>
								<th class="w3-center" style="width:10%">Cat.</th>
							</tr>
						</thead>
						<tbody id="liste_combats_tapis_{@ID}">

							<xsl:apply-templates select="Combat">
								<xsl:sort select="@Numero" data-type="number"/>
							</xsl:apply-templates>

							<xsl:if test="count(Combat) = 0">
								<tr>
									<td colspan="5" class="w3-center w3-padding-large w3-text-grey">
										<i>Aucun combat en attente</i>
									</td>
								</tr>
							</xsl:if>

						</tbody>
					</table>
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="Combat">

		<tr id="combat_{@ID}" data-row-index="{position()}" class="combat-row">

			<xsl:variable name="isActive" select="position() = 1"/>

			<xsl:variable name="cardColorClass">
				<xsl:choose>
					<xsl:when test="$isActive">w3-pale-green</xsl:when>
					<xsl:otherwise>w3-white</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<td class="w3-center w3-xxlarge">
				<xsl:value-of select="position()"/>
			</td>

			<td class="w3-center w3-xlarge">
				<xsl:value-of select="@Numero"/>
			</td>

			<td style="padding: 4px 8px;">
				<div class="w3-card w3-round-small {$cardColorClass}" style="overflow:hidden;">

					<header class="w3-container w3-light-grey" style="padding: 5px 10px; display:flex; align-items:center;">
						<span class="badge-color w3-white w3-text-black w3-border w3-border-grey" style="margin-right:10px; flex-shrink:0;">B</span>

						<div style="flex-grow:1; line-height:1.1em;">
							<span class="judoka-nom">
								<xsl:value-of select="Judoka[@Role='Blanc']/@Nom"/>
							</span>
							<br/>
							<span class="judoka-prenom">
								<xsl:value-of select="Judoka[@Role='Blanc']/@Prenom"/>
							</span>
						</div>
					</header>

					<footer class="w3-container" style="padding: 4px 10px;">
						<span class="judoka-club" style="display:block; text-align:right;">
							<xsl:value-of select="Judoka[@Role='Blanc']/@Club"/>
						</span>
					</footer>
				</div>
			</td>

			<td style="padding: 4px 8px;">
				<div class="w3-card w3-round-small {$cardColorClass}" style="overflow:hidden;">

					<header class="w3-container w3-light-grey" style="padding: 5px 10px; display:flex; align-items:center;">
						<span class="badge-color w3-blue w3-border w3-border-black" style="margin-right:10px; flex-shrink:0;">B</span>

						<div style="flex-grow:1; line-height:1.1em;">
							<span class="judoka-nom">
								<xsl:value-of select="Judoka[@Role='Bleu']/@Nom"/>
							</span>
							<br/>
							<span class="judoka-prenom">
								<xsl:value-of select="Judoka[@Role='Bleu']/@Prenom"/>
							</span>
						</div>
					</header>

					<footer class="w3-container" style="padding: 4px 10px;">
						<span class="judoka-club" style="display:block; text-align:right;">
							<xsl:value-of select="Judoka[@Role='Bleu']/@Club"/>
						</span>
					</footer>
				</div>
			</td>

			<td class="w3-center w3-small">
				<xsl:value-of select="@Categorie"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>