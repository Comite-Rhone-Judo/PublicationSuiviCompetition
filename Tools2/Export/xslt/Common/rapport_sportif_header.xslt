<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" />
  <xsl:param name="style"></xsl:param>

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
        RAPPORT SPORTIF
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

        <div class="col3">
          <span style="font-size:0.7em;vertical-align: bottom;">
            <xsl:value-of select="concat(substring(//competition[1]/@date, 1, 2), '-', substring(//competition[1]/@date, 3, 2), '-',substring(//competition[1]/@date, 5))"/>
          </span>
          <xsl:if test="//image[@type=2]/@image" >
            <img border="0" align="center" height="100px">
              <xsl:attribute name="src">
                <xsl:value-of select="//image[@type=2]/@image"/>
              </xsl:attribute>
              <xsl:text>&#32;</xsl:text>
            </img>
          </xsl:if>
        </div>

        <div class="col2" style="height:100px;">
          <span style="text-decoration:underline;">
            RAPPORT SPORTIF
          </span>
          <br/>
          <span>
            <xsl:value-of select="//competition[1]/titre"/>
            <xsl:text disable-output-escaping="yes">&#32;-&#32;</xsl:text>
            <xsl:value-of select="//competition[1]/lieu"/>
          </span>
          <br/>
          <br/>
        </div>
      </div>
    </body>
  </xsl:template>
</xsl:stylesheet>