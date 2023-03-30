<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" indent="yes" />
  <xsl:param name="style"></xsl:param>

  <xsl:variable name="typeCompetition" select="/competition/@type">
  </xsl:variable>

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
          file:///<xsl:value-of select="$style"/>style_rapport_selection.css
        </xsl:attribute>
      </link>
      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>
    </head>
    <body>
      <div>
        <table class="t2">
         

          <xsl:if test="$typeCompetition = '1'">
            <thead>
              <tr>
                <th>Q</th>
                <th class="name">
                 Equipe
                </th>
                <th class="name">
                  Club
                </th>
                <th class="name">
                  Comite
                </th>
                <th class="name">
                  Ligue
                </th>
              </tr>
            </thead>
            <xsl:apply-templates select="//equipes/equipe">
            </xsl:apply-templates>
          </xsl:if>

          <xsl:if test="$typeCompetition != '1'">
            <thead>
              <tr>
                <th>Q</th>
                <th class="name">
                  NOM et Prénom
                </th>
                <th class="name">
                  Club
                </th>
                <th class="name">
                  Comite
                </th>
                <th class="name">
                  Ligue
                </th>
              </tr>
            </thead>
            <xsl:apply-templates select="//judokas/judoka">
            </xsl:apply-templates>
          </xsl:if>
          
         
        </table>
      </div>
    </body>
  </xsl:template>

  <xsl:template match="judoka">
    <xsl:variable name="club" select="@club"/>   
    <xsl:variable name="position" select="position()"/>
    <tr height="50px">
      <xsl:attribute name="class">
        <xsl:if test="$position mod 2 = 0">
          <xsl:text>alter</xsl:text>
        </xsl:if>
      </xsl:attribute>
      <td class="align_center">
        <xsl:value-of select="@qualifie1"/>
        <!--<input type="checkbox" >
          <xsl:if test= "@qualifie='true'">
            <xsl:attribute name="checked"></xsl:attribute>
          </xsl:if>
        </input>-->
      </td>
      <td>
        <xsl:value-of select="@nom"/>
        <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
        <xsl:value-of select="@prenom"/>
      </td>
      <td>
        <xsl:value-of select="@clubnom"/>
      </td>
      <td>
        <xsl:value-of select="@comitenomcourt"/>
      </td>
      <td>
        <xsl:value-of select="@liguenomcourt"/>
      </td>
      
    </tr>
  </xsl:template>

  <xsl:template match="equipe">
    <xsl:variable name="club1" select="@club"/>
    <xsl:variable name="comite1" select="@comite"/>
    <xsl:variable name="ligue1" select="@ligue"/>    
   
    <xsl:variable name="position" select="position()"/>
    <tr height="50px">
      <xsl:attribute name="class">
        <xsl:if test="$position mod 2 = 0">
          <xsl:text>alter</xsl:text>
        </xsl:if>
      </xsl:attribute>
      <td class="align_center">
        <xsl:value-of select="descendant::*[1]/@qualifie1"/>       
      </td>
      <td>
        <xsl:value-of select="@nom"/>        
      </td>
      <td>
        <xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
      </td>
      <td>
        <xsl:value-of select="$comite1"/>
      </td>
      <td>
        <xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
      </td>

    </tr>
  </xsl:template>
</xsl:stylesheet>