<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="xml" indent="yes" />
  <xsl:param name="style"></xsl:param>


  <xsl:template match="/">
    <html>
      <xsl:apply-templates/>
    </html>
  </xsl:template>

  <xsl:template match="/*">
    <head>
      <meta charset="utf-8" />
      <title>JUDO</title>

      <script type="text/javascript" >       
          window.location.href='../common/menu.html';
        <!--window.location.href='http://<xsl:value-of select="./@address"/>:<xsl:value-of select="./@port"/>/site/<xsl:value-of select="./@titre"/>/common/feuille_combats_tapis_All.html';-->
      </script>
    </head>
    <body>     
    </body>
  </xsl:template>
</xsl:stylesheet>
