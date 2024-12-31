<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="xml" indent="yes"/>
  <xsl:param name="style"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>


	<xsl:variable name="apos">'</xsl:variable>
	
	<xsl:template match="/">
    <html>
      <xsl:apply-templates/>
    </html>
  </xsl:template>

  <xsl:template match="/*">
    <head>
      <meta charset="utf-8"/>
      <title>Suivi Compétition</title>

      <script type="text/javascript">
		  <xsl:value-of disable-output-escaping="yes" select="concat('window.location.href = ', $apos, $commonPath, 'avancement.html', $apos, ';')"/>
       </script>
    </head>
    <body> </body>
  </xsl:template>
</xsl:stylesheet>
