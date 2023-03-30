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
    </html>
  </xsl:template>

  <xsl:template name="header">
      <tr>
        <th>Licence</th>
        <th>Nom	et prénom</th>
        <th>Date de naissance</th>
        <th>Club</th>
        <th>Dépt</th>
        <th>Ligue</th>
      </tr>
  </xsl:template>

  <xsl:template name="sexe">
    <xsl:param name="sexe"/>
    <xsl:choose>
      <xsl:when test="$type = 1">
        <xsl:for-each select="//judoka[@lib_sexe = $sexe and generate-id(.)=generate-id(key('epreuves', @idepreuve)[1])]">
          <!--<xsl:sort select="@idepreuve"/>-->
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

                <xsl:if test="count(//equipe)>0">
                  <xsl:call-template name="equipe"/>
                </xsl:if>

                <xsl:if test="count(//equipe)=0">
                  <xsl:call-template name="judoka"  />
                </xsl:if>

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
                
                <xsl:if test="count(//equipe)>0">
                  <xsl:call-template name="equipe"/>
                </xsl:if>

                <xsl:if test="count(//equipe)=0">
                  <xsl:call-template name="judoka"  />
                </xsl:if>
                
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

                <xsl:if test="count(//equipe)>0">
                  <xsl:call-template name="equipe"/>
                </xsl:if>

                <xsl:if test="count(//equipe)=0">
                  <xsl:call-template name="judoka"  />
                </xsl:if>

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

              <xsl:if test="count(//equipe)>0">
                <xsl:call-template name="equipe"/>
              </xsl:if>

              <xsl:if test="count(//equipe)=0">
                <xsl:call-template name="judoka"  />
              </xsl:if>

            </xsl:for-each>
          </table>
        </div>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template name="equipe">
    <tr height="50px">
      <td>

      </td>
      <td>
        <xsl:value-of select="@nom"/>
      </td>
      <td>
      </td>
      <td>
        <xsl:value-of select="@club"/>
      </td>
      <td>
        <xsl:value-of select="@comite"/>
      </td>
      <td>
        <xsl:value-of select="@ligue"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template name="judoka">
    <tr height="50px">
      <td>
        <xsl:value-of select="@licence"/>
      </td>
      <td>
        <xsl:value-of select="@nom"/> <xsl:value-of select="@prenom"/>
      </td>
      <td>
        <xsl:value-of select="@naissance"/>
      </td>
      <td>
        <xsl:value-of select="@clubnomcourt"/>
      </td>
      <td>
        <xsl:value-of select="@comitenomcourt"/>
      </td>
      <td>
        <xsl:value-of select="@liguenomcourt"/>
      </td>
    </tr>
  </xsl:template>  
  
</xsl:stylesheet>