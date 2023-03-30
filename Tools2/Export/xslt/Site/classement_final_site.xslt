<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="html" indent="yes" />
  <xsl:param name="style"></xsl:param>
  <!--<xsl:param name="menu"></xsl:param>-->
  <xsl:param name="js"></xsl:param>

  <xsl:template match="/">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html>
      <xsl:apply-templates/>
    </html>
  </xsl:template>

  <xsl:template match="/*">
    <head>
      <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
      <!--<meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no" />-->
      <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
      <meta http-equiv="Pragma" content="no-cache" />
      <meta http-equiv="Expires" content="0" />
      
      <link type="text/css" rel="stylesheet" href="../style/style_classement.css" ></link>
      <link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

      <!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->
     
      
      
      <!--<xsl:variable name="css1">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_classement.css</xsl:text>
      </xsl:variable>

      <xsl:variable name="css2">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_menu.css</xsl:text>
      </xsl:variable>

      <style type="text/css">
        <xsl:value-of select="document($css1)" disable-output-escaping="yes" />
        <xsl:value-of select="document($css2)" disable-output-escaping="yes" />
      </style>

      -->

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body>

      <div class="btn_defilement">
        <div style="position: fixed;right: 10px;top: 10px;">
          <a class="btn btn-danger" onclick="anim2();"  style="margin-right: 10px;">Actualiser</a>
          <a class="btn btn-info" onclick="setDefilement();">Défilement</a>
        </div>
      </div>

      <div class="btn_menu">
        <a class="btn btn-warning" href="../common/menu.html" role="button">Menu</a>
      </div>

      <div class="panel panel-primary">
        <div class="panel-heading clearfix" style="text-align:center;">
          
          <span style="font-size:1.0em;">
            <xsl:value-of select="./titre"/>
            <xsl:text> - </xsl:text>
            <xsl:value-of select="./lieu"/>
          </span>
          <br/>
          <span style="font-size:1.2em;">
            <!--<xsl:text> (</xsl:text>-->
            <!--<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
                      <xsl:text> </xsl:text>-->
            <xsl:value-of select="//epreuve[1]/@nom"/>
            <!--<xsl:text>)</xsl:text>-->
          </span>
        
        </div>



        <div class="panel-body">
          <div class="table-responsive">
            <table class="table">
              <thead>
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
              </thead>
              <xsl:apply-templates select="//classement/participant">
                <xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
              </xsl:apply-templates>
            </table>
          </div>
        </div>
      </div>

      <!--<div class="bandeau">
        <div class="header1" >
          <xsl:value-of select="./titre"/>
          <xsl:text> - </xsl:text>
          <xsl:value-of select="./lieu"/>
        </div>
        <div class="header2" >
          <xsl:value-of select="//epreuve[1]/@nom_cateage"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="//epreuve[1]/@nom"/>
        </div>
      </div>-->
      <!--<div id='newsbox' style='width: 100%; height:95%;'>
        <div id='newslist' style='visibility: hidden; width: 100%'>-->

      <!--<div class="main">
        <table class="classement" style="margin: auto auto auto auto;">


        </table>
      </div>-->
      <!--</div>
      </div>-->
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
        <xsl:if test="$position mod 2 = 1">
          <xsl:text>active</xsl:text>
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
        <xsl:value-of select="$clubN/nomCourt"/>
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