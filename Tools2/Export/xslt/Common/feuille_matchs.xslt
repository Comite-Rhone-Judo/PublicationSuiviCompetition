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

  <xsl:variable name="typeCompetition" select="/competition/@type">
  </xsl:variable>

  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[
         public string date_tostring(string d1)
         {
            if(String.IsNullOrWhiteSpace(d1))
            {
                return "";
            }
            return (DateTime.ParseExact(d1, "ddMMyyyy HHmmss", null)).ToString(@"HH\:mm");
         }
         
          public string tableau(string niveau, string reference)
          {
            if(String.IsNullOrWhiteSpace(niveau))
            {
              return "";
            }
            
            int niv = int.Parse(niveau);
            if(niv < 1)
            {
              return "Repéchage " + reference;
            }
            else if(niv == 1)
            {
              return "Finale";
            }            
            else if(niv == 2)
            {
              return "Demi-finale";
            }
            
            niv = (int)Math.Pow(2, int.Parse(niveau) - 1);
            return "" + niv + " ème";
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
          file:///<xsl:value-of select="$style"/>style_combats.css
        </xsl:attribute>
        <xsl:text>&#32;</xsl:text>
      </link>

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body>

      <xsl:for-each select="//tapis[count(./combats/combat[score[@judoka][1] and score[@judoka][2] and (not(@vainqueur) or @vainqueur = '-1')]) &gt; 0]">
        <xsl:variable name="tapis" select="@tapis" />
        <div class="poule">
          <xsl:if test="position() != 1" >
            <xsl:attribute name="style">
              page-break-before : always;
            </xsl:attribute>
          </xsl:if>

          <xsl:for-each select="//tapis[@tapis=$tapis]/combats/combat">
            <xsl:sort select="@time_programmation" data-type="number" order="ascending"/>

            <xsl:if test="$typeCompetition = '1'">
              <xsl:call-template name="combatEqu" >
                <xsl:with-param name="combat" select="."/>
              </xsl:call-template>
            </xsl:if>

            <xsl:if test="$typeCompetition != '1'">
              <xsl:call-template name="combatInd" >
                <xsl:with-param name="combat" select="."/>
              </xsl:call-template>
            </xsl:if>
          </xsl:for-each>
        </div>
      </xsl:for-each>

      <xsl:apply-templates select="//tapis"/>
    </body>
  </xsl:template>

  <xsl:template name="combatInd">
    <xsl:param name="combat"/>

    <xsl:variable name="epreuve" select="$combat/@epreuve" />
    <xsl:variable name="phase" select="$combat/@phase" />

    <div style="page-break-inside:avoid;">
      <hr class="filet1" />
      <div class="header1" >

        <xsl:value-of select="//epreuve[@ID = $epreuve]/@nom_cateage"/>
        <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
        <xsl:value-of select="//epreuve[@ID = $epreuve]/@sexe"/>
        <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
        <xsl:value-of select="//epreuve[@ID = $epreuve]/@nom_catepoids"/>
        <xsl:text disable-output-escaping="yes">&#032;</xsl:text>


        <xsl:variable name="typephase" select="//phase[@id=$phase]/@typePhase"/>
        <xsl:choose>
          <xsl:when test= "$typephase = '1'">
            (POULE
            <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
            <xsl:value-of select="$combat/@reference"/>
            <xsl:text disable-output-escaping="yes">)</xsl:text>
          </xsl:when>

          <xsl:when test= "$typephase = '2'">
            (TABLEAU
            <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
            <xsl:value-of select="cs:tableau($combat/@niveau, $combat/@reference)" />
            <xsl:text disable-output-escaping="yes">)</xsl:text>
          </xsl:when>
        </xsl:choose>

      </div>

      <table class="t1">
        <tr>
          <td>
            Arbitre : <!--<xsl:value-of select="@reference"/>-->
          </td>
          <td>
            Tapis : <xsl:value-of select="ancestor::tapis/@tapis"/>
          </td>
          <td class="right">

            <xsl:variable name="tmax" select="concat($combat/@date_programmation, ' ', $combat/@time_programmation)"/>

            Heure : <xsl:value-of select="cs:date_tostring($tmax)" />
            <!--<xsl:value-of select="concat(substring(@time_programmation, 1, 2), ':', substring(@time_programmation, 3))"/>-->
          </td>
        </tr>
      </table>

      <xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
      <xsl:variable name="judoka1" select="//participant[@judoka=$participant1]/descendant::*[1]"/>
      <xsl:variable name="club1" select="$judoka1/@club"/>
      <xsl:variable name="comite1" select="//club[@ID=$club1]/@comite"/>
      <xsl:variable name="ligue1" select="//club[@ID=$club1]/@ligue"/>
	  <xsl:variable name="pays1" select="$judoka1/@pays"/>

      <xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
      <xsl:variable name="judoka2" select="//participant[@judoka=$participant2]/descendant::*[1]"/>
      <xsl:variable name="club2" select="$judoka2/@club"/>
      <xsl:variable name="comite2" select="//club[@ID=$club2]/@comite"/>
      <xsl:variable name="ligue2" select="//club[@ID=$club2]/@ligue"/>
	  <xsl:variable name="pays2" select="$judoka2/@pays"/>

      <div class="d1">
        <table style="width:100%; border-collapse: collapse;">
          <tr >
            <!--<td rowspan="2">
              <xsl:if test= "position() &lt; 10">
                0<xsl:value-of select="position()"/>
              </xsl:if>
              <xsl:if test= "position() &gt; 9">
                <xsl:value-of select="position()"/>
              </xsl:if>
            </td>-->
            <td class="combatname">
              <xsl:value-of select="$judoka1/@nom"/>
              <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
              <xsl:value-of select="$judoka1/@prenom"/>
            </td>
            <td class="combatclub">
                  <xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
                  <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
                  <xsl:value-of select="$comite1"/>
                  <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>

                  <xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
				  <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
				  <xsl:value-of select="//pays[@code=$pays1]/@abrF"/>
            </td>
            <td class="cellres1">I</td>
            <td class="cellres1">W</td>
            <td class="cellres1"></td>
            <td class="cellres1 shido">S</td>
            <td class="cellres1">WIN</td>
          </tr>
          <tr>
            <td class="hr combatname">
              <xsl:value-of select="$judoka2/@nom"/>
              <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
              <xsl:value-of select="$judoka2/@prenom"/>
            </td>
            <td class="hr combatclub">
				<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
				<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
				<xsl:value-of select="$comite2"/>
				<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>

				<xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
				<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
				<xsl:value-of select="//pays[@code=$pays2]/@abrF"/>
            </td>
            <td class="cellres2">I</td>
            <td class="cellres2">W</td>
            <td class="cellres2"></td>
            <td class="cellres2 shido">S</td>
            <td class="cellres2">WIN</td>
          </tr>
        </table>
      </div>
      <!--<hr class="filet1" />-->
      <!--<hr class="filet" />-->
    </div >

  </xsl:template>

  <xsl:template name="combatEqu">
    <xsl:param name="combat"/>

    <xsl:variable name="epreuve" select="$combat/@epreuve" />
    <xsl:variable name="phase" select="$combat/@phase" />
    <xsl:variable name="typephase" select="//phase[@id=$phase]/@typePhase"/>
    <xsl:variable name="ecartement" select="//phase[@id=$phase]/@ecartement"/>

    <xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
    <xsl:variable name="judoka1" select="//participant[@judoka=$participant1]/descendant::*[1]"/>
    <xsl:variable name="club1" select="$judoka1/@club"/>
    <xsl:variable name="comite1" select="$judoka1/@comite"/>
    <xsl:variable name="ligue1" select="$judoka1/@ligue"/>
	<xsl:variable name="pays1" select="$judoka1/@pays"/>

    <xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
    <xsl:variable name="judoka2" select="//participant[@judoka=$participant2]/descendant::*[1]"/>
    <xsl:variable name="club2" select="$judoka2/@club"/>
    <xsl:variable name="comite2" select="$judoka2/@comite"/>
    <xsl:variable name="ligue2" select="$judoka2/@ligue"/>
	<xsl:variable name="pays2" select="$judoka2/@pays"/>


    <div style="page-break-inside:avoid;">


      <div class="header1" >

        <xsl:value-of select="//epreuve_equipe[@ID = $epreuve]/@libelle"/>

        <xsl:choose>
          <xsl:when test= "$typephase = '1'">
            <xsl:text disable-output-escaping="yes">(POULE&#032;</xsl:text>
            <xsl:value-of select="$combat/@reference"/>
            <xsl:text disable-output-escaping="yes">)</xsl:text>
          </xsl:when>

          <xsl:when test= "$typephase = '2'">
            <xsl:text disable-output-escaping="yes">(TABLEAU&#032;</xsl:text>
            <xsl:value-of select="cs:tableau($combat/@niveau, $combat/@reference)" />
            <xsl:text disable-output-escaping="yes">)</xsl:text>
          </xsl:when>
        </xsl:choose>

        <br/>

        <xsl:value-of select="$judoka1/@nom"/>
        <xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
        <xsl:value-of select="$judoka2/@nom"/>
      </div>

      <table class="t1">
        <tr>
          <td>
            Arbitre : <!--<xsl:value-of select="@reference"/>-->
          </td>
          <td>
            Tapis : <xsl:value-of select="ancestor::tapis/@tapis"/>
          </td>
          <td class="right">

            <xsl:variable name="tmax" select="concat($combat/@date_programmation, ' ', $combat/@time_programmation)"/>

            Heure : <xsl:value-of select="cs:date_tostring($tmax)" />
            <!--<xsl:value-of select="concat(substring(@time_programmation, 1, 2), ':', substring(@time_programmation, 3))"/>-->
          </td>
        </tr>
      </table>

      <xsl:for-each select="$combat/rencontre">
        <xsl:sort select="@catePoids" data-type="number" order="ascending"/>

        <xsl:variable name="rencontre" select="."/>

        <div>
          <hr class="filet1" />

          <div class="d1">
            <table style="width:100%; border-collapse: collapse;">
              <tr >
                <td rowspan="2" class="cellres0">
                  <xsl:value-of select="//epreuve[@ID=$rencontre/@catePoids and @categoriePoids]/@nom"/>                
                </td>
                <td class="combatname cellres1">
                  <xsl:value-of select="$judoka1/@nom"/>
                  <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
                  <xsl:value-of select="$judoka1/@prenom"/>
                </td>
                <td class="cellres1">I</td>
                <td class="cellres1">W</td>
                <td class="cellres1"></td>
                <td class="cellres1 shido">S</td>
                <td class="cellres1">WIN</td>
              </tr>
              <tr>
                <td class="hr combatname cellres2">
                  <xsl:value-of select="$judoka2/@nom"/>
                  <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
                  <xsl:value-of select="$judoka2/@prenom"/>
                </td>

                <td class="cellres2">I</td>
                <td class="cellres2">W</td>
                <td class="cellres2"></td>
                <td class="cellres2 shido">S</td>
                <td class="cellres2">WIN</td>
              </tr>
            </table>
          </div>
          <!--<hr class="filet" />-->
        </div >
      </xsl:for-each>
    </div>


  </xsl:template>
</xsl:stylesheet>