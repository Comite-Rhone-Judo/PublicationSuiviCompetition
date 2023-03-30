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

  <xsl:variable name="typeCompetition" select="/competition/@type">
  </xsl:variable>

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
      <xsl:apply-templates select="//phase/poules/poule"/>
    </body>
  </xsl:template>

  <xsl:template match="poule">
    <xsl:variable name="numero" select="@numero"/>
    <div class="poule" style="page-break-inside: avoid;" >
      <div>

        <div class="poule_title">
          <div id='poule_heure_div'>
            <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
          </div>
          <div id='poule_tapis_div'>
            Tapis <xsl:value-of select="../../combats/combat[@reference=$numero][1]/@tapis"/>
          </div>
          <div id='poule_title_div'>
            <xsl:value-of select="@nom"/>
          </div>
        </div>

        <div>
          <table class="t2">
            <tr>
              <th></th>
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
              <th>V</th>
              <xsl:if test="$typeCompetition = '1'">
                <th>V Ind</th>
              </xsl:if>

              <th>P</th>
            </tr>
            <xsl:apply-templates select="../../participants/participant[@poule=$numero]">
              <xsl:sort select="@classementFinal"/>
              <xsl:with-param name="poule" select="$numero" />
            </xsl:apply-templates>
          </table>
        </div>
      </div>
    </div>
  </xsl:template>


  <xsl:template match="participant">
    <xsl:param name="poule" />

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
        <img>
          <xsl:if test= "@qualifie='true'">
            <xsl:attribute name="src">
              file:///<xsl:value-of select="$style"/>icon/checked_checkbox-32.png
            </xsl:attribute>
          </xsl:if>
          <xsl:if test= "not(@qualifie='true')">
            <xsl:attribute name="src">
              file:///<xsl:value-of select="$style"/>icon/unchecked_checkbox-32.png
            </xsl:attribute>
          </xsl:if>
        </img>
      </td>
      <td class="align_center">
        <xsl:value-of select="@classementFinal"/>
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
        <xsl:value-of select="$clubN/@comite"/>
      </td>
      <td>
        <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
      </td>
      <td>
      </td>
      <td>
        <xsl:value-of select="@nbVictoires"/>
      </td>
      <xsl:if test="$typeCompetition = '1'">
        <td>
          <xsl:value-of select="@nbVictoiresInd"/>
        </td>
      </xsl:if>
      <td>
        <xsl:value-of select="@cumulPoints"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>