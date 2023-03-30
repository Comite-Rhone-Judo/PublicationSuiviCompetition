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
          file:///<xsl:value-of select="$style"/>footer.css
        </xsl:attribute>
      </link>

      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>
      <div class="main">

        <table>
          <tr>
            <xsl:for-each select="//image[@type=0]">
              <td>
                <img border="0" align="center" height="80px">
                  <xsl:attribute name="src">
                    <xsl:value-of select="./@image"/>
                  </xsl:attribute>
                  <xsl:text>&#32;</xsl:text>
                </img>
              </td>
            </xsl:for-each>
          </tr>
        </table>
      </div>
    </body>
  </xsl:template>
</xsl:stylesheet>