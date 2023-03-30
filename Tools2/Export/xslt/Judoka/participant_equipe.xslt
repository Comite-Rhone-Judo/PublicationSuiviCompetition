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
          file:///<xsl:value-of select="$style"/>style_judokas.css
        </xsl:attribute>
      </link>

      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>
      <div>
        <table class="t2">
          <thead>
            <tr>
              <th>Licence</th>
              <th>Nom	et prénom</th>
              <th>Date de naissance</th>
              <th>Club</th>
              <th>Dépt</th>
              <th>Ligue</th>
            </tr>
          </thead>

          <xsl:if test="count(//equipe)>0">
            <xsl:apply-templates select="//equipe"/>
          </xsl:if>

          <xsl:if test="count(//equipe)=0">
            <xsl:apply-templates select="//judoka"/>
          </xsl:if>


        </table>
      </div>
    </body>
  </xsl:template>

  <xsl:template match="equipe">
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

  <xsl:template match="judoka">
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