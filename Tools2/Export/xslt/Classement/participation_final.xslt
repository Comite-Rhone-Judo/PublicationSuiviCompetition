<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" />
  <xsl:param name="style"></xsl:param>
  <xsl:param name="fond"></xsl:param>

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
          file:///<xsl:value-of select="$style"/>style_diplome.css
        </xsl:attribute>
      </link>
      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>

      <xsl:for-each select="//inscrits/judoka">
        <xsl:sort select="@nom" data-type="text" order="ascending"/>

        <div class="div_max">
              <div class="div_img">
                <img >
                  <xsl:attribute name="src">
                    file:///<xsl:value-of select="$fond"/>_participation.jpg
                    file:///<xsl:value-of select="$style"/>icon/cadre-sunbum-1_2467263-XL.png
                  </xsl:attribute>
                </img>
              </div>
              <div class="div_competition">
                <xsl:value-of select="//competition[1]/titre"/>
              </div>

              <xsl:variable name="j1" select="." />

              <div class="div_nom">
                <xsl:value-of select="$j1/@nom"/>
                <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
                <xsl:value-of select="$j1/@prenom"/>
              </div>

              <div class="div_competition">
                <xsl:value-of select="//competition[1]/titre"/>
              </div>

              <div class="div_club">

                <xsl:variable name="club" select="$j1/@club"/>
                <xsl:variable name="clubN" select="//club[@ID=$club]"/>

                <xsl:value-of select="$clubN/nom"/>
              </div>

              <!--<div class="div_cate">
                <xsl:value-of select="ancestor::epreuve[1]/@nom"/>
              </div>-->

              <div class="div_date">
                <xsl:variable name="compet" select="/competition" />
                <xsl:value-of select="concat(substring($compet/@date, 1, 2), '-', substring($compet/@date, 3, 2), '-',substring($compet/@date, 5))"/>
              </div>




              <!--<xsl:if test="//image[@type=1]/@image" >
                <div class="div_logo1">
                  <img>
                    <xsl:attribute name="src">
                      <xsl:value-of select="//image[@type=1]/@image"/>
                    </xsl:attribute>
                    <xsl:text>&#32;</xsl:text>
                  </img>
                </div>
              </xsl:if>
              <xsl:if test="//image[@type=2]/@image" >
                <div class="div_logo2">
                  <img>
                    <xsl:attribute name="src">
                      <xsl:value-of select="//image[@type=2]/@image"/>
                    </xsl:attribute>
                    <xsl:text>&#32;</xsl:text>
                  </img>
                </div>
              </xsl:if>-->

              <!--<div class="div_logo3">

                <table>
                  <tr>
                    <xsl:for-each select="//image[@type=0]">
                      <td>
                        <img>
                          <xsl:attribute name="src">
                            <xsl:value-of select="./@image"/>
                          </xsl:attribute>
                          <xsl:text>&#32;</xsl:text>
                        </img>
                      </td>
                    </xsl:for-each>
                  </tr>
                </table>
              </div>-->


            </div>
         
      </xsl:for-each>


    </body>
  </xsl:template>
</xsl:stylesheet>