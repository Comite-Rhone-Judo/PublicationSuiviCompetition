<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="xml" indent="yes"/>
    <xsl:param name="style"/>
	<xsl:param name="SiteRoutes"/>


	<xsl:variable name="apos">'</xsl:variable>
	<xsl:variable name="urlAvancement" select="$SiteRoutes/*/@urlAvancement"/>
	
	<xsl:template match="/">
    <html>
		<xsl:apply-templates select="docroot"/>
    </html>
  </xsl:template>

  <xsl:template match="docroot">
    <head>
      <meta charset="utf-8"/>
      <title>Suivi Compétition</title>

      <script type="text/javascript">
		  <xsl:value-of disable-output-escaping="yes" select="concat('window.location.href = ', $apos, $urlAvancement, $apos, ';')"/>
       </script>
    </head>
    <body> </body>
  </xsl:template>
</xsl:stylesheet>
