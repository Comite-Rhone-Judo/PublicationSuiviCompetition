<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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


	<xsl:template match="/*">
		<!-- ENTETE HTML -->
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
			<meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no" />
			<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
			<meta http-equiv="Pragma" content="no-cache" />
			<meta http-equiv="Expires" content="0" />
			<link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

			<script type="text/javascript" >
				<xsl:value-of select="$js"/>
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<!-- LES BOUTONS -->
			<div class="btn_defilement">
				<div style="position: fixed;right: 10px;top: 10px;">
					<a class="btn btn-danger" onclick="anim2();"  style="margin-right: 10px;">Actualiser</a>
					<a class="btn btn-info" onclick="setDefilement();">Défilement</a>
				</div>
			</div>
			<!-- VUE AFFECTATION -->
			<div class="col-md-12">
				<xsl:for-each select="/competitions/competition">
					<xsl:if test="count(./epreuve) > 0">
						<div class="col-md-12">
							<div class="panel panel-info" style="text-align: center;">
								<div class="panel-heading clearfix">
									<span id="ContentPlaceHolder1_Label2" style="width: 100%;">
										<xsl:value-of select="./titre"/>
									</span>
								</div>

								<div class="panel-body">
									<xsl:if test="count(./epreuve[@sexe='F']) > 0">
										<div class="row">
											<div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
												Catégories féminines
											</div>
											<xsl:apply-templates select="./epreuve[@sexe='F']"/>
										</div>
									</xsl:if>

									<xsl:if test="count(./epreuve[@sexe='M']) > 0">
										<div class="row">
											<div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
												Catégories masculines
											</div>
											<xsl:apply-templates select="./epreuve[@sexe='M']"/>
										</div>
									</xsl:if>

									<xsl:if test="count(./epreuve[not(@sexe)]) > 0">
										<div class="row">
											<xsl:apply-templates select="./epreuve[not(@sexe)]"/>
										</div>
									</xsl:if>
								</div>
							</div>
						</div>
					</xsl:if>
				</xsl:for-each>
			</div>
			<!--</div>-->
		</body>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- AFFECTATION TAPIS -->
	<xsl:template  match="epreuve">
		<div class="col-lg-6">
			<div class="panel panel-warning" role="alert" style="padding: 5px; text-align:left;">
				<div class="panel-heading">
					<xsl:value-of select="./@libelle"/>
					<xsl:value-of select="./@nom"/>
				</div>
				<div class="panel-body">
					<xsl:choose>
						<xsl:when test="count(./TapisEpreuve/tapis) > 1">
							<xsl:text>Tapis&#32;</xsl:text>
							<xsl:for-each select="./TapisEpreuve/tapis">
								<xsl:sort select="./@no_tapis"/>
								<xsl:value-of select="./@no_tapis"/>
								<xsl:if test="position() &lt; last() - 1">
									<xsl:text>, </xsl:text>
								</xsl:if>
								<xsl:if test="position()=last() - 1">
									<xsl:text> et </xsl:text>
								</xsl:if>
							</xsl:for-each>
						</xsl:when>
						<xsl:when test="count(./TapisEpreuve/tapis) = 1">
							<xsl:text>Tapis&#32;</xsl:text>
							<xsl:for-each select="./TapisEpreuve/tapis">
								<xsl:sort select="./@no_tapis"/>
								<xsl:value-of select="./@no_tapis"/>
							</xsl:for-each>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>&#160;</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</div>
		</div>
	</xsl:template>
</xsl:stylesheet>