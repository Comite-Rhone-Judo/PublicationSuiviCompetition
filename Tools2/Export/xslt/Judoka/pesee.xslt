<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>

	<xsl:param name="type"></xsl:param>

	<xsl:key name="epreuves" match="judoka" use="@idepreuve"/>
	<xsl:key name="ligues" match="judoka" use="@ligue"/>
	<xsl:key name="comites" match="judoka" use="@comitenomcourt"/>
	<xsl:key name="clubs" match="judoka" use="@club"/>

	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

	<xsl:template match="/">
		<html>

			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

				<link rel="stylesheet" type="text/css" >
					<xsl:attribute name="href">
						file:///<xsl:value-of select="$style"/>style_pesee.css
					</xsl:attribute>
				</link>

				<title>
					<xsl:value-of select="//epreuve[1]/@nom"/>
				</title>
			</head>
			<body>
				<div>
					<xsl:call-template name="sexe">
						<xsl:with-param name="sexe" >
							<xsl:text >F</xsl:text>
						</xsl:with-param>
					</xsl:call-template>
					<xsl:call-template name="sexe">
						<xsl:with-param name="sexe" >
							<xsl:text >M</xsl:text>
						</xsl:with-param>
					</xsl:call-template>

				</div>
			</body>

			<!--<xsl:apply-templates/>-->
		</html>
	</xsl:template>

	<xsl:template name="header">
		<tr>
			<th style="width:15%;" >Observations</th>
			<th style="width:15%;" >Signature</th>
			<xsl:if test="$typeCompetition = '3'">
				<th>
					<xsl:text disable-output-escaping="yes">PTS</xsl:text>
				</th>
			</xsl:if>
			<th style="width:10%;" >Poids</th>
			<xsl:if test="$typeCompetition = '2'">
				<th>Qualifié</th>
			</xsl:if>
			<th style="padding-right: 5%;">Nom et prénom</th>
			<th  style="padding-right: 1%;">Naissance</th>
			<th>Grade</th>
			<th style="padding-right: 5%;">Club</th>
			<!--<th>Dépt</th>
      <th>Ligue</th>-->
		</tr>
	</xsl:template>

	<xsl:template name="sexe">
		<xsl:param name="sexe"/>
		<xsl:choose>
			<xsl:when test="$type = 1">
				<xsl:for-each select="//judoka[@lib_sexe = $sexe and generate-id(.)=generate-id(key('epreuves', @idepreuve)[1])]">
					<!--<xsl:sort select="@idepreuve" data-type="number"/>-->
					<div class="class1">
						<table class="t2">
							<thead>
								<tr>
									<th colspan="9" style="text-align:center;">
										<xsl:text disable-output-escaping="no"> Epreuve : </xsl:text>
										<xsl:value-of select="@libepreuve" />
									</th>
								</tr>
								<xsl:call-template name="header"  />
							</thead>
							<xsl:for-each select="key('epreuves', @idepreuve)">
								<xsl:sort select="@nom"/>
								<xsl:sort select="@prenom"/>
								<xsl:call-template name="judoka"  />
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</xsl:when>
			<xsl:when test="$type = 2">
				<xsl:for-each select="//judoka[@lib_sexe = $sexe and generate-id(.)=generate-id(key('ligues', @ligue)[1])]">
					<xsl:sort select="@ligue"/>
					<div class="class1">
						<table class="t2">
							<thead>
								<tr>
									<th colspan="9" style="text-align:center;">
										<xsl:text disable-output-escaping="no"> Ligue : </xsl:text>
										<xsl:value-of select="@liguenom" />
									</th>
								</tr>
								<xsl:call-template name="header"  />
							</thead>
							<xsl:for-each select="key('ligues', @ligue)">
								<xsl:sort select="@nom"/>
								<xsl:sort select="@prenom"/>
								<xsl:call-template name="judoka"  />
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</xsl:when>
			<xsl:when test="$type = 3">
				<xsl:for-each select="//judoka[@lib_sexe = $sexe and generate-id(.)=generate-id(key('comites', @comitenomcourt)[1])]">
					<xsl:sort select="@comitenomcourt"/>
					<div class="class1">
						<table class="t2">
							<thead>
								<tr>
									<th colspan="9" style="text-align:center;">
										<xsl:text disable-output-escaping="no"> Comité : </xsl:text>
										<xsl:value-of select="@comitenom" />
									</th>
								</tr>
								<xsl:call-template name="header"  />
							</thead>
							<xsl:for-each select="key('comites', @comitenomcourt)">
								<xsl:sort select="@nom"/>
								<xsl:sort select="@prenom"/>
								<xsl:call-template name="judoka"  />
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</xsl:when>
			<xsl:when test="$type = 4">
				<xsl:for-each select="//judoka[@lib_sexe = $sexe and generate-id(.)=generate-id(key('clubs', @club)[1])]">
					<xsl:sort select="@club"/>
					<div class="class1">
						<table class="t2">
							<thead>
								<tr>
									<th colspan="9" style="text-align:center;">
										<xsl:text disable-output-escaping="no"> Club : </xsl:text>
										<xsl:value-of select="@clubnom" />
									</th>
								</tr>
								<xsl:call-template name="header"  />
							</thead>
							<xsl:for-each select="key('clubs', @club)">
								<xsl:sort select="@nom"/>
								<xsl:sort select="@prenom"/>
								<xsl:call-template name="judoka"  />
							</xsl:for-each>
						</table>
					</div>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<div class="class1">
					<table class="t2">
						<thead>
							<xsl:call-template name="header"  />
						</thead>
						<xsl:for-each select="//judoka[@lib_sexe = $sexe]">
							<xsl:sort select="@nom"/>
							<xsl:sort select="@prenom"/>
							<xsl:call-template name="judoka"  />
						</xsl:for-each>
					</table>
				</div>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>


	<xsl:template name="judoka">
		<tr >
			<td></td>
			<td></td>
			<xsl:if test="$typeCompetition = '3'">
				<td>
					<xsl:value-of select="@points"/>
				</td>
			</xsl:if>
			<td style="padding-left: 3%;">
				<xsl:if test="@poidsMesure != '00.0'">
					<xsl:value-of select="@poidsMesure"/>
				</xsl:if>
			</td>
			<xsl:if test="$typeCompetition = '2'">
				<td class="left">
					<xsl:if test="@observation = 5">
						<xsl:attribute name="style">
							<xsl:text disable-output-escaping="yes">font-style: italic;font-weight: bold;</xsl:text>
						</xsl:attribute>
					</xsl:if>
					<xsl:choose>
						<xsl:when test="@serie2 = 1">
							<xsl:text disable-output-escaping="no">Q1</xsl:text>
						</xsl:when>
						<xsl:when test="@serie2 = 2">
							<xsl:text disable-output-escaping="no">Q2</xsl:text>
						</xsl:when>
						<xsl:when test="@observation = 5">
							<xsl:text disable-output-escaping="no">R</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text disable-output-escaping="no">Q</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</xsl:if>

			<td>
				<xsl:if test="@observation = 5">
					<xsl:attribute name="style">
						<xsl:text disable-output-escaping="yes">font-style: italic;font-weight: bold;</xsl:text>
					</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="@nom"/> <xsl:value-of select="@prenom"/>
			</td>
			<td style="padding-right: 1%;">
				<xsl:value-of select="@naissance"/>
			</td>
			<td>
				<xsl:variable name="ceinture" select="@ceinture_id"/>
				<xsl:choose>
					<xsl:when test= "//ceinture[@id=$ceinture]/@nom != ''">
						<xsl:value-of select="//ceinture[@id=$ceinture]/@nom"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text disable-output-escaping="yes">&#160;</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:value-of select="@clubnomcourt"/> - <xsl:value-of select="@comitenomcourt"/> - <xsl:value-of select="@liguenomcourt"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>