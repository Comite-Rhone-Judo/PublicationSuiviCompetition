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
          file:///<xsl:value-of select="$style"/>style_feuille_classement.css
        </xsl:attribute>
      </link>
      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>
      <hr class="filet" />
      <div>
        <table class="t2">
          <tr>
            <th></th>
            <th>
              NOM et Prénom
            </th>
            <th>
              Club
            </th>
            <th>
              Comité
            </th>
            <th>
              Ligue
            </th>
            <th>
              Pays
            </th>
          </tr>
          <xsl:apply-templates select="//classement/participant">
            <xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
          </xsl:apply-templates>
        </table>
      </div>

    </body>
  </xsl:template>


  <xsl:template match="participant">
    <xsl:variable name="participant1" select="@judoka" />
    <xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />



    <xsl:variable name="club" select="$j1/@club"/>
    <xsl:variable name="clubN" select="//club[@ID=$club]"/>
    <xsl:variable name="comite" select="$clubN/@comite"/>
    <xsl:variable name="ligue" select="$clubN/@ligue"/>
    <xsl:variable name="position" select="position()"/>



    <tr>
      <xsl:attribute name="class">
        <xsl:if test="$position mod 2 = 0">
          <xsl:text>alter</xsl:text>
        </xsl:if>
      </xsl:attribute>
      <td class="align_center">
        <xsl:choose>
          <xsl:when test="@classementFinal != 0 and @classementFinal &lt; 9">
            <xsl:value-of select="@classementFinal"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>NC</xsl:text>
          </xsl:otherwise>
        </xsl:choose>

      </td>
      <td>
        <xsl:value-of select="$j1/@nom"/>
        <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
        <xsl:value-of select="$j1/@prenom"/>
      </td>
      <td>
        <xsl:value-of select="$clubN/nom"/>
      </td>
      <td>
        <xsl:value-of select="$comite"/>
      </td>
      <td>
        <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
      </td>
      <td>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>