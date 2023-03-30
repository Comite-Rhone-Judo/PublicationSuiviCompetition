<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:ms="urn:schemas-microsoft-com:xslt"
                xmlns:dt="urn:schemas-microsoft-com:datatypes">

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
          file:///<xsl:value-of select="$style"/>
          <xsl:text>style_rapport.css</xsl:text>
        </xsl:attribute>
        <xsl:text>&#32;</xsl:text>
      </link>

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body>


      <xsl:for-each select="//competition">
        <div class="compet">
          <xsl:if test="position() != count(//competition)">
            <xsl:attribute name="style">page-break-before : always;</xsl:attribute>
          </xsl:if>

          <div class="name">
            <xsl:value-of select="@titre"/>
          </div>
          <div class="name">
            Le
            <xsl:text>&#32;</xsl:text>
            <xsl:value-of select="concat(substring(@date, 1, 2), '-', substring(@date, 3, 2), '-',substring(@date, 5))"/>
          </div>
          <div class="name">
            <xsl:value-of select="@lieu"/>
          </div>

          <div class="name">
          </div>

          <div class="title">BILAN</div>
          
          <div style="width:90%; margin: auto;">
            <div class="col1">

              <div class="title">Masculins</div>
          
                <table class="recap_present">               
                <xsl:for-each select="./epreuves/epreuve[@sexe='M']">
                  <xsl:variable name="pos" select="position()"/>
                  <tr>
                    <td class="recap_present_td">
                      <div class="name">
                        <xsl:value-of select="@nom_catepoids"/>
                      </div>                      
                    </td>
                    <td class="recap_present_td">
                      <div class="name">
                        <xsl:value-of select="count(./inscrits/judoka[@present='true'])"/>
                        <xsl:text>&#32;présent(s)</xsl:text>                      
                      </div>                      
                    </td>
                  </tr>
                </xsl:for-each>
                <tr>
                  <td class="recap_present_td total">
                    <div class="name">
                      <xsl:text>TOTAL</xsl:text>
                    </div>
                    
                  </td>
                  <td class="recap_present_td total">
                    <div class="name">
                      <xsl:value-of select="count(./epreuves/epreuve[@sexe='M']/inscrits/judoka[@present='true'])"/>
                      <xsl:text>&#32;présent(s)</xsl:text>
                    </div>                    
                  </td>
                </tr>
              </table>
            </div>
            <div class="col3">

              <div class="title">Féminines</div>
              
              <table class="recap_present">               
                <xsl:for-each select="./epreuves/epreuve[@sexe='F']">
                  <xsl:variable name="pos" select="position()"/>
                  <tr>
                    <td class="recap_present_td">
                      <div class="name">
                        <xsl:value-of select="@nom_catepoids"/>
                      </div>
                    </td>
                    <td class="recap_present_td">
                      <div class="name">
                        <xsl:value-of select="count(./inscrits/judoka[@present='true'])"/>
                        <xsl:text>&#32;présent(s)</xsl:text>
                      </div>
                      
                    </td>
                  </tr>
                </xsl:for-each>
                <tr>
                  <td class="recap_present_td total">
                    <div class="name">
                      <xsl:text>TOTAL</xsl:text>
                    </div>                    
                  </td>
                  <td class="recap_present_td total">
                    <div class="name">
                      <xsl:value-of select="count(./epreuves/epreuve[@sexe='F']/inscrits/judoka[@present='true'])"/>
                      <xsl:text>&#32;présent(s)</xsl:text>
                    </div>
                   
                  </td>
                </tr>
              </table>
            </div>
            <div class="spacer" style="clear: both;">
              <xsl:text>&#32;</xsl:text>
            </div>
          </div>

          <xsl:if test="count(./epreuves/epreuve[@sexe='F']) &gt; 0">

            <div class="title">RESULTATS FEMININES</div>

            <xsl:for-each select="./epreuves/epreuve[@sexe='F']">
              <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>
              <table class="recap_classement">
                <tr>
                  <td class="recap_classement_td total" width="10%">
                    <xsl:value-of select="@nom_catepoids"/>
                  </td>
                  <td class="recap_classement_td total" width="15%">NOM</td>
                  <td class="recap_classement_td total" width="15%">PRENOM</td>
                  <td class="recap_classement_td total" width="30%">CLUB</td>
                  <td class="recap_classement_td total" width="15%">DEPT</td>
                  <td class="recap_classement_td total" width="15%">LIGUE</td>
                  <td class="recap_classement_td total" width="15%">PAYS</td>
                </tr>

                <xsl:for-each select="./phases/phase[@suivant=0]/participants/participant">
                  <xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
                  <xsl:if test="@classementFinal &lt; 8">
                    <tr>
                      <xsl:variable name="participant" select="."/>
                      <xsl:variable name="judoka" select="//participants/participant[@judoka=$participant/@ID]/descendant::*[1]"/>
                      <xsl:variable name="club" select="$judoka/@club"/>
                      <xsl:variable name="comite" select="//club[@ID=$club]/@comite"/>
                      <xsl:variable name="ligue" select="//club[@ID=$club]/@ligue"/>
                      <xsl:variable name="pays" select="$judoka/@pays"/>

                      <td class="recap_classement_td total">
                        <xsl:value-of select="@classementFinal"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@prenom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//club[@ID=$club]/nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$comite"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//pays[@ID=$pays]/@abr3"/>
                      </td>
                    </tr>
                  </xsl:if>
                </xsl:for-each>
              </table>
            </xsl:for-each>

          </xsl:if>

          <xsl:if test="count(./epreuves/epreuve[@sexe='M']) &gt; 0">
            <div class="title">RESULTATS MASCULINS</div>

            <xsl:for-each select="./epreuves/epreuve[@sexe='M']">
              <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>

              <table class="recap_classement">
                <tr>
                  <td class="recap_classement_td total" width="10%">
                    <xsl:value-of select="@nom_catepoids"/>
                  </td>
                  <td class="recap_classement_td total" width="15%">NOM</td>
                  <td class="recap_classement_td total" width="15%">PRENOM</td>
                  <td class="recap_classement_td total" width="30%">CLUB</td>
                  <td class="recap_classement_td total" width="15%">DEPT</td>
                  <td class="recap_classement_td total" width="15%">LIGUE</td>
                  <td class="recap_classement_td total" width="15%">PAYS</td>
                </tr>

                <xsl:for-each select="./phases/phase[@suivant=0]/participants/participant">
                  <xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
                  <xsl:if test="@classementFinal &lt; 8">
                    <tr>
                      <xsl:variable name="participant" select="."/>
                      <xsl:variable name="judoka" select="//participants/participant[@judoka=$participant/@ID]/descendant::*[1]"/>
                      <xsl:variable name="club" select="$judoka/@club"/>
                      <xsl:variable name="comite" select="//club[@ID=$club]/@comite"/>
                      <xsl:variable name="ligue" select="//club[@ID=$club]/@ligue"/>
                      <xsl:variable name="pays" select="$judoka/@pays"/>

                      <td class="recap_classement_td total">
                        <xsl:value-of select="@classementFinal"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@prenom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//club[@ID=$club]/nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$comite"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//pays[@ID=$pays]/@abr3"/>
                      </td>
                    </tr>
                  </xsl:if>
                </xsl:for-each>
              </table>
            </xsl:for-each>
          </xsl:if>

          <xsl:if test="count(./epreuves/epreuve[not(@sexe)]) &gt; 0">
            <div class="title">RESULTATS</div>

            <xsl:for-each select="./epreuves/epreuve[not(@sexe)]">
              <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>

              <table class="recap_classement">
                <tr>
                  <td class="recap_classement_td total" width="10%">
                    <xsl:value-of select="@nom_catepoids"/>
                  </td>
                  <td class="recap_classement_td total" width="15%">NOM</td>
                  <td class="recap_classement_td total" width="15%">PRENOM</td>
                  <td class="recap_classement_td total" width="30%">CLUB</td>
                  <td class="recap_classement_td total" width="15%">DEPT</td>
                  <td class="recap_classement_td total" width="15%">LIGUE</td>
                  <td class="recap_classement_td total" width="15%">PAYS</td>
                </tr>

                <xsl:for-each select="./phases/phase[@suivant=0]/participants/participant">
                  <xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
                  <xsl:if test="@classementFinal &lt; 8">
                    <tr>
                      <xsl:variable name="participant" select="."/>
                      <xsl:variable name="judoka" select="//participants/participant[@judoka=$participant/@ID]/descendant::*[1]"/>
                      <xsl:variable name="club" select="$judoka/@club"/>
                      <xsl:variable name="comite" select="//club[@ID=$club]/@comite"/>
                      <xsl:variable name="ligue" select="//club[@ID=$club]/@ligue"/>
                      <xsl:variable name="pays" select="$judoka/@pays"/>

                      <td class="recap_classement_td total">
                        <xsl:value-of select="@classementFinal"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$judoka/@prenom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//club[@ID=$club]/nom"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="$comite"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
                      </td>
                      <td class="recap_classement_td">
                        <xsl:value-of select="//pays[@ID=$pays]/@abr3"/>
                      </td>
                    </tr>
                  </xsl:if>
                </xsl:for-each>
              </table>
            </xsl:for-each>
          </xsl:if>

        </div>
      </xsl:for-each>
    </body>
  </xsl:template>




</xsl:stylesheet>