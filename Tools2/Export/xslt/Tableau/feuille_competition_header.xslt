<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
                 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:ms="urn:schemas-microsoft-com:xslt"
                xmlns:dt="urn:schemas-microsoft-com:datatypes"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:cs="urn:cs">

	<xsl:output method="xml" indent="yes" />
	<xsl:param name="style"></xsl:param>

	<msxsl:script language="C#" implements-prefix="cs">
		<![CDATA[
         public string time_string(string d1)
         {
            if(String.IsNullOrWhiteSpace(d1))
            {
                return "";
            }
            return (DateTime.ParseExact(d1, "ddMMyyyy HHmmss", null)).ToString(@"hh\:mm");
         }
         
         public string date_string(string d1)
         {
            if(String.IsNullOrWhiteSpace(d1))
            {
                return "";
            }
            return (DateTime.ParseExact(d1, "ddMMyyyy HHmmss", null)).ToString(@"dd\-MM\-yyyy");
         }
     ]]>
	</msxsl:script>

	<xsl:variable name="repechage">
		<xsl:text>false</xsl:text>
	</xsl:variable>
	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

	<xsl:template match="/">
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>

	<xsl:template match="/*">
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

			<link rel="stylesheet" type="text/css" >
				<xsl:attribute name="href">
					file:///<xsl:value-of select="$style"/>header.css
				</xsl:attribute>
			</link>

			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
			</title>




		</head>
		<body>

			<div class="main">
				<xsl:if test="//image[@type=1]/@image" >
					<div class="col1">
						<img border="0" align="center" height="100px">
							<xsl:attribute name="src">
								<xsl:value-of select="//image[@type=1]/@image"/>
							</xsl:attribute>
							<xsl:text>&#32;</xsl:text>
						</img>
					</div>
				</xsl:if>
				<xsl:if test="//image[@type=2]/@image" >
					<div class="col3">
						<img border="0" align="center" height="100px">
							<xsl:attribute name="src">
								<xsl:value-of select="//image[@type=2]/@image"/>
							</xsl:attribute>
							<xsl:text>&#32;</xsl:text>
						</img>
					</div>
				</xsl:if>
				<div class="col2" style="height:100px;">
					<span>
						<xsl:value-of select="./titre"/>
						<xsl:text>&#32;-&#32;</xsl:text>
						<xsl:value-of select="./lieu"/>
						<br/>
						<xsl:text>TABLEAU</xsl:text>
						<xsl:text>&#32;(Le&#32;</xsl:text>

						<xsl:variable name="date" select="./@date">
						</xsl:variable>

						<xsl:value-of select="concat(substring(@date, 1, 2), '-', substring(@date, 3, 2), '-',substring(@date, 5))" />
						<xsl:text>)</xsl:text>

						<!--<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
            <xsl:text>&#32;</xsl:text>
            <xsl:value-of select="//epreuve[1]/@sexe"/>
            <xsl:text>&#32;</xsl:text>
            <xsl:value-of select="//epreuve[1]/@nom_catepoids"/>
            <xsl:text>&#32;(</xsl:text>
            <xsl:value-of select="count(//participant)"/>
            <xsl:text>&#32;combattants)</xsl:text>-->
					</span>
				</div>
			</div>
			<!--<div class="epreuve_c">
				<table>
					<tr>
						<td style="font-size: 1.3em;">
							<xsl:if test="$typeCompetition != '1'">
								<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
								<xsl:text>&#32;</xsl:text>
								<xsl:value-of select="//epreuve[1]/@sexe"/>
								<xsl:text>&#32;</xsl:text>
							</xsl:if>
						</td>
					</tr>
					<tr>
						<xsl:if test="$typeCompetition != '1'">
							<td style="font-size: 2.5em;">
								<xsl:if test="not(contains(//epreuve[1]/@nom, '+'))">
									<xsl:text disable-output-escaping="yes">-</xsl:text>
									<xsl:value-of select="//epreuve[1]/@nom"/>
								</xsl:if>
								<xsl:if test="contains(//epreuve[1]/@nom, '+')">
									<xsl:value-of select="//epreuve[1]/@nom"/>
								</xsl:if>
							</td>
						</xsl:if>
						<xsl:if test="$typeCompetition = '1'">
							<td style="font-size: 2.5em;">
								<xsl:value-of select="//epreuve[1]/@nom"/>
							</td>
						</xsl:if>
					</tr>
					<tr>
						<td style="font-size: 1.3em;">
							<xsl:if test="//phase[1]/@isequipe='false' ">
								<xsl:value-of select="count(//participant)"/>
								<xsl:text disable-output-escaping="yes">&#32;Judokas</xsl:text>
							</xsl:if>
							<xsl:if test="//phase[1]/@isequipe='true' ">
								<xsl:value-of select="count(//participant)"/>
								<xsl:text disable-output-escaping="yes">&#32;Equipes</xsl:text>
							</xsl:if>
						</td>
					</tr>
				</table>
			</div>-->
		</body>
	</xsl:template>
</xsl:stylesheet>