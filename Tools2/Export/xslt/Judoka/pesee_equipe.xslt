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
          file:///<xsl:value-of select="$style"/>style_pesee_equipe.css
        </xsl:attribute>
      </link>

      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>
      <xsl:apply-templates select="//equipe"/>
    </body>
  </xsl:template>

  <xsl:template match="equipe">
    <div class="class1">
      <table class="t1">
        <tr>
          <td>
            Nom de l'équipe:
            <xsl:value-of select="@nom" />
          </td>
          <td>Nom et signature de l'accompagnateur</td>
        </tr>
      </table>

      <table class="t2">
        <thead>
          <tr>
            <th colspan="4" class="noborder"></th>
            <th class="border1"></th>
            <th colspan="2">1er tour</th>
            <th colspan="2">2eme Tour</th>
            <th colspan="2">3eme Tour</th>
            <th colspan="2">4eme Tour</th>
            <th colspan="2">5eme Tour</th>
            <th colspan="2">6eme Tour</th>
            <th colspan="2">7eme Tour</th>
            <th>Signature</th>
          </tr>
          <tr>
            <th colspan="4" class="border1"></th>
            <th>Equipes rencontrées</th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th colspan="2"></th>
            <th>Si anomalie(s)</th>
          </tr>
          <tr>
            <th>Cat</th>
            <th>Poids</th>
            <th>D.A</th>
            <th>Etr</th>
            <th>Nom et prénom judoka</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th>V ou D</th>
            <th>P</th>
            <th></th>
          </tr>
        </thead>
        <xsl:apply-templates select="./judoka[@etat=4]">
          <xsl:sort order="descending" select="@libepreuve" data-type="number" />
        </xsl:apply-templates>
        <tr>
          <td colspan="4" class="border1"></td>
          <td>Victoires/Points</td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
        </tr>
        <tr>
          <td colspan="4" class="border1"></td>
          <td>Vainqueur</td>
          <td colspan="2"></td>
          <td colspan="2"></td>
          <td colspan="2"></td>
          <td colspan="2"></td>
          <td colspan="2"></td>
          <td colspan="2"></td>
          <td colspan="2"></td>
        </tr>

      </table>
    </div>
  </xsl:template>

  <xsl:template match="judoka">
    <tr>
      <td class="td1">
        <xsl:value-of select="@libepreuve"/>
      </td>
      <td class="td1"></td>
      <td class="td1"></td>
      <td class="td1"></td>
      <td class="td2">

        <xsl:choose>
          <xsl:when test="@nom = 'Judoka 1'">

          </xsl:when>
          <xsl:when test="@nom = 'Judoka 2'">

          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="@nom"/> <xsl:value-of select="@prenom"/>
          </xsl:otherwise>
        </xsl:choose>

      </td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td3"></td>
      <td class="td2"></td>
    </tr>
  </xsl:template>

</xsl:stylesheet>