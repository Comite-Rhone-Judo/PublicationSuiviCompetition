<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:ms="urn:schemas-microsoft-com:xslt"
                xmlns:dt="urn:schemas-microsoft-com:datatypes"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:cs="urn:cs">

  <xsl:output method="xml" indent="yes" />
  <xsl:param name="style"></xsl:param>

  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[
         public string diff_date(string d1, string d2)
         {
            if(String.IsNullOrWhiteSpace(d1) || String.IsNullOrWhiteSpace(d2))
            {
                return "";
            }
            return (DateTime.ParseExact(d1, "ddMMyyyy HHmmss", null) - DateTime.ParseExact(d2, "ddMMyyyy HHmmss", null)).ToString(@"hh\:mm");
         }
         
         public string diff_date2(string d1, string d2)
         {
         if(String.IsNullOrWhiteSpace(d1) || String.IsNullOrWhiteSpace(d2))
            {
                return "";
            }
         //return DateTime.ParseExact(d2, "ddMMyyyy HHmmss", null).ToString("HH:mmss");
            return (DateTime.ParseExact(d1, "ddMMyyyy HHmmss", null) - DateTime.ParseExact(d2, "ddMMyyyy HHmmss", null)).ToString(@"hh\:mm");
         }
     ]]>
  </msxsl:script>

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

          <div class="title">Délégué</div>


          <xsl:for-each select="//delegue">
            <div class="name">
              <xsl:value-of select="@fonction"/><xsl:text>&#32;</xsl:text>:<xsl:text>&#32;</xsl:text><xsl:value-of select="./nom"/><xsl:text>&#32;</xsl:text><xsl:value-of select="./prenom"/>
            </div>
          </xsl:for-each>

          <div class="title">Arbitrage</div>

          <div style="width:90%; margin: auto;">
            <div class="col1">
              <div class="name">
                <xsl:text>Responsable&#32;de&#32;arbitres&#32;:&#32;</xsl:text>
                <xsl:for-each select="//arbitres/arbitre[@responsable='true']">
                  <xsl:value-of select="@nom"/>
                  <xsl:text>&#32;</xsl:text>
                  <xsl:value-of select="@prenom"/>
                  <br/>
                </xsl:for-each>
              </div>

              <div class="title">
                <xsl:text>LISTE&#32;DES&#32;ARBITRES</xsl:text>
              </div>
              <xsl:for-each select="//arbitres/arbitre[@responsable='false']">
                <div class="name">
                  <xsl:value-of select="position()"/>
                  <xsl:text>.&#32;</xsl:text>
                  <xsl:value-of select="@nom"/>
                  <xsl:text>&#32;</xsl:text>
                  <xsl:value-of select="@prenom"/>
                </div>
              </xsl:for-each>
            </div>

            <div class="col3">
              <div class="name">
                <xsl:text>Responsable&#32;des&#32;C.S.&#32;:&#32;</xsl:text>
                <xsl:for-each select="//commissaires/commissaire[@responsable='true']">
                  <xsl:value-of select="@nom"/>
                  <xsl:text>&#32;</xsl:text>
                  <xsl:value-of select="@prenom"/>
                  <br/>
                </xsl:for-each>
              </div>

              <div class="title">
                <xsl:text>LISTE&#32;DES&#32;C.S.</xsl:text>
              </div>
              <xsl:for-each select="//commissaires/commissaire[@responsable='false']">
                <div class="name">
                  <xsl:value-of select="position()"/>
                  <xsl:text>.&#32;</xsl:text>
                  <xsl:value-of select="@nom"/>
                  <xsl:text>&#32;</xsl:text>
                  <xsl:value-of select="@prenom"/>
                </div>
              </xsl:for-each>
            </div>
            <div class="spacer" style="clear: both;">
              <xsl:text>&#32;</xsl:text>
            </div>
          </div>

          <xsl:if test="count(./epreuves/epreuve[@sexe='M']) &gt; 0">
            <div class="title">
              masculins
            </div>

            <table class="recap_epreuve">
              <tr>
                <th class="recap_epreuve_th total">
                  épreuve
                </th>
                <th class="recap_epreuve_th total">
                  début de la pesée
                </th>
                <th class="recap_epreuve_th total">
                  heure de début des poules
                </th>
                <th class="recap_epreuve_th total">
                  heure de fin des poules
                </th>
                <th class="recap_epreuve_th total">
                  Tps de combat des poules de la<br/>catégorie
                </th>

                <th class="recap_epreuve_th total">
                  heure de début du tableau
                </th>
                <th class="recap_epreuve_th total">
                  heure de fin du tableau
                </th>
                <th class="recap_epreuve_th total">
                  Tps total tableau
                </th>
                <th class="recap_epreuve_th total">
                  Tps total de la catégorie
                </th>
                <th class="recap_epreuve_th total">
                  Tps total de la catégorie pesée comprise
                </th>
              </tr>
              <xsl:for-each select="./epreuves/epreuve[@sexe='M']">
                <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>

                <tr>

                  <!--EPREUVE-->
                  <td class="recap_epreuve_td">
                    <xsl:value-of select="@nom_catepoids"/>
                  </td>
                  <!--PESEE-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="count(./inscrits/judoka) != 0">
                      <xsl:for-each select="./inscrits/judoka">
                        <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <!--<xsl:value-of select="ms:format-time(@datePesee, 'HH:mm')"/>-->

                          <xsl:value-of select="concat(substring(@time_datePesee, 1, 2), ':', substring(@time_datePesee, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <!--DEBUT POULE-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <!--FIN POULE-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <!--DUREE POULE-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">

                      <xsl:variable name="tmin">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:variable name="tmax">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                    </xsl:if>
                  </td>
                  <!--DEBUT TABLEAU-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <!--FIN TABLEAU-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <!--DUREE TABLEAU-->
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:variable name="tmin">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:variable name="tmax">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                    </xsl:if>
                  </td>
                  <!--DUREE CATE SANS PASEE-->
                  <td class="recap_epreuve_td">
                    <xsl:variable name="tmin">
                      <xsl:if test="count(./phases/phase[@typePhase=1]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                      <xsl:if test="count(./phases/phase[@typePhase=1]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                      <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                    </xsl:variable>

                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                  </td>
                  <!--DUREE CATE AVEC PASEE-->
                  <td class="recap_epreuve_td">
                    <xsl:variable name="tmin">
                      <xsl:if test="count(./inscrits/judoka) != 0">
                        <xsl:for-each select="./inscrits/judoka">
                          <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <!--<xsl:value-of select="@datePesee"/>-->
                            <xsl:value-of select="concat(@date_datePesee, ' ', @time_datePesee)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                      <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>
                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />

                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>

          <xsl:if test="count(./epreuves/epreuve[@sexe='F']) &gt; 0">
          <div class="title">
            féminines
          </div>

          <table class="recap_epreuve">
            <tr>
              <th class="recap_epreuve_th total">
                épreuve
              </th>
              <th class="recap_epreuve_th total">
                début de la pesée
              </th>
              <th class="recap_epreuve_th total">
                heure de début des poules
              </th>
              <th class="recap_epreuve_th total">
                heure de fin des poules
              </th>
              <th class="recap_epreuve_th total">
                Tps de combat des poules de la<br/>catégorie
              </th>

              <th class="recap_epreuve_th total">
                heure de début du tableau
              </th>
              <th class="recap_epreuve_th total">
                heure de fin du tableau
              </th>
              <th class="recap_epreuve_th total">
                Tps total tableau
              </th>
              <th class="recap_epreuve_th total">
                Tps total de la catégorie
              </th>
              <th class="recap_epreuve_th total">
                Tps total de la catégorie pesée comprise
              </th>
            </tr>
            <xsl:for-each select="./epreuves/epreuve[@sexe='F']">
              <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>
              <tr>
                <td class="recap_epreuve_td">
                  <xsl:value-of select="@nom_catepoids"/>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="count(./inscrits/judoka) != 0">
                    <xsl:for-each select="./inscrits/judoka">
                      <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                      <xsl:if test="position() = 1">
                        <!--<xsl:value-of select="ms:format-time(@datePesee, 'HH:mm')"/>-->
                        <xsl:value-of select="concat(substring(@time_datePesee, 1, 2), ':', substring(@time_datePesee, 3, 2))"/>
                      </xsl:if>
                    </xsl:for-each>
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=1]">
                    <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                      <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                      <xsl:if test="position() = 1">
                        <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                      </xsl:if>
                    </xsl:for-each>
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=1]">
                    <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                      <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                      <xsl:if test="position() = 1">
                        <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                      </xsl:if>
                    </xsl:for-each>
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=1]">

                    <xsl:variable name="tmin">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:variable>

                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=2]">
                    <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                      <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                      <xsl:if test="position() = 1">
                        <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                      </xsl:if>
                    </xsl:for-each>
                  </xsl:if>
                </td>

                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=2]">
                    <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                      <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                      <xsl:if test="position() = 1">
                        <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                      </xsl:if>
                    </xsl:for-each>
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:if test="./phases/phase[@typePhase=2]">
                    <xsl:variable name="tmin">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:variable>

                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                  </xsl:if>
                </td>
                <td class="recap_epreuve_td">
                  <xsl:variable name="tmin">
                    <xsl:if test="count(./phases/phase[@typePhase=1]) = 0">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                    <xsl:if test="count(./phases/phase[@typePhase=1]) != 0">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </xsl:variable>

                  <xsl:variable name="tmax">
                    <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>

                    <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </xsl:variable>

                  <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                </td>
                <td class="recap_epreuve_td">
                  <xsl:variable name="tmin">
                    <xsl:if test="count(./inscrits/judoka) != 0">
                      <xsl:for-each select="./inscrits/judoka">
                        <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <!--<xsl:value-of select="@datePesee"/>-->
                          <xsl:value-of select="concat(@date_datePesee, ' ', @time_datePesee)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </xsl:variable>

                  <xsl:variable name="tmax">
                    <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>

                    <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </xsl:variable>
                  <xsl:value-of select="cs:diff_date($tmax, $tmin)" />

                </td>
              </tr>
            </xsl:for-each>
          </table>
          </xsl:if>

          <xsl:if test="count(./epreuves/epreuve[not(@sexe)]) &gt; 0">

            <table class="recap_epreuve">
              <tr>
                <th class="recap_epreuve_th total">
                  épreuve
                </th>
                <th class="recap_epreuve_th total">
                  début de la pesée
                </th>
                <th class="recap_epreuve_th total">
                  heure de début des poules
                </th>
                <th class="recap_epreuve_th total">
                  heure de fin des poules
                </th>
                <th class="recap_epreuve_th total">
                  Tps de combat des poules de la<br/>catégorie
                </th>

                <th class="recap_epreuve_th total">
                  heure de début du tableau
                </th>
                <th class="recap_epreuve_th total">
                  heure de fin du tableau
                </th>
                <th class="recap_epreuve_th total">
                  Tps total tableau
                </th>
                <th class="recap_epreuve_th total">
                  Tps total de la catégorie
                </th>
                <th class="recap_epreuve_th total">
                  Tps total de la catégorie pesée comprise
                </th>
              </tr>
              <xsl:for-each select="./epreuves/epreuve[not(@sexe)]">
                <xsl:sort select="@poidsMin" data-type="number" order="ascending"/>
                <tr>
                  <td class="recap_epreuve_td">
                    <xsl:value-of select="@nom_catepoids"/>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="count(./inscrits/judoka) != 0">
                      <xsl:for-each select="./inscrits/judoka">
                        <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <!--<xsl:value-of select="ms:format-time(@datePesee, 'HH:mm')"/>-->
                          <xsl:value-of select="concat(substring(@time_datePesee, 1, 2), ':', substring(@time_datePesee, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">
                      <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=1]">

                      <xsl:variable name="tmin">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:variable name="tmax">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_debut, 1, 2), ':', substring(@time_debut, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>

                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                        <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                        <xsl:if test="position() = 1">
                          <xsl:value-of select="concat(substring(@time_fin, 1, 2), ':', substring(@time_fin, 3, 2))"/>
                        </xsl:if>
                      </xsl:for-each>
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:if test="./phases/phase[@typePhase=2]">
                      <xsl:variable name="tmin">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:variable name="tmax">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:variable>

                      <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                    </xsl:if>
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:variable name="tmin">
                      <xsl:if test="count(./phases/phase[@typePhase=1]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                      <xsl:if test="count(./phases/phase[@typePhase=1]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_debut" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_debut, ' ', @time_debut)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                      <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>

                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />
                  </td>
                  <td class="recap_epreuve_td">
                    <xsl:variable name="tmin">
                      <xsl:if test="count(./inscrits/judoka) != 0">
                        <xsl:for-each select="./inscrits/judoka">
                          <xsl:sort select="@time_datePesee" data-type="text" order="ascending"/>
                          <xsl:if test="position() = 1">
                            <!--<xsl:value-of select="@datePesee"/>-->
                            <xsl:value-of select="concat(@date_datePesee, ' ', @time_datePesee)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>

                    <xsl:variable name="tmax">
                      <xsl:if test="count(./phases/phase[@typePhase=2]) != 0">
                        <xsl:for-each select="./phases/phase[@typePhase=2][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>

                      <xsl:if test="count(./phases/phase[@typePhase=2]) = 0">
                        <xsl:for-each select="./phases/phase[@typePhase=1][1]/combats/combat[score[1]/@judoka != 0 and score[2]/@judoka != 0]">
                          <xsl:sort select="@time_fin" data-type="text" order="descending"/>
                          <xsl:if test="position() = 1">
                            <xsl:value-of select="concat(@date_fin, ' ', @time_fin)"/>
                          </xsl:if>
                        </xsl:for-each>
                      </xsl:if>
                    </xsl:variable>
                    <xsl:value-of select="cs:diff_date($tmax, $tmin)" />

                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
        </div>
      </xsl:for-each>

      <div class="compet">
        <div class="title">Commentaires</div>


        <xsl:for-each select="//delegue">
          <div class="name">
            <xsl:value-of select="@fonction"/><xsl:text>&#32;</xsl:text>:<xsl:text>&#32;</xsl:text><xsl:value-of select="./commentaire"/>
          </div>
        </xsl:for-each>
      </div>
    </body>
  </xsl:template>




</xsl:stylesheet>