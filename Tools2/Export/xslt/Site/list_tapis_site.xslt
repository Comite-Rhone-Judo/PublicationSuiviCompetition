<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <!--<xsl:param name="site_url"></xsl:param>-->

  <xsl:output method="html" indent="yes" />
  <xsl:param name="style"></xsl:param>
  <!--<xsl:param name="menu"></xsl:param>-->
  <xsl:param name="js"></xsl:param>

  <xsl:key name="combats" match="combat" use="@niveau"/>


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
      <link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

      <!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->



      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>

      <!--<xsl:variable name="css1">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_menu.css</xsl:text>
      </xsl:variable>

      <style type="text/css">
        <xsl:value-of select="document($css1)" disable-output-escaping="yes" />
      </style>

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>-->

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body>

      <!--<div class="panel panel-primary">
        <div class="panel-body">-->
          <div class="col-md-12">
            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
              Liste des tapis
            </div>
          </div>
          <xsl:for-each select="/competitions/competition[1]/tapis[@tapis != '0']">
            <!--c-->
            <xsl:sort select="@tapis" data-type="number" order="ascending"/>
            <div class="col-md-6">
              <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
                <a class="alert-link">
                  <xsl:attribute name="href">
                    <!--<xsl:value-of select="$site_url"/>-->
                    <xsl:text>..</xsl:text>
                    <xsl:text>/common/tapis_</xsl:text>
                    <xsl:value-of select="@tapis"/>
                    <xsl:text>.html</xsl:text>
                  </xsl:attribute>
                  <xsl:text>Tapis&#32;</xsl:text>
                  <xsl:value-of select="@tapis"/>
                </a>
              </div>
            </div>
          </xsl:for-each>
        <!--</div>
      </div>-->
    </body>
  </xsl:template>


  <xsl:template match="epreuve">
    <xsl:param name="type" />

    <xsl:if test="$type = 0">
      <xsl:call-template name="se_prepare_epreuve">
        <xsl:with-param name="epreuve" select="."/>
      </xsl:call-template>
    </xsl:if>

    <xsl:if test="$type = 1">
      <xsl:call-template name="avancement_epreuve">
        <xsl:with-param name="epreuve" select="."/>
      </xsl:call-template>
    </xsl:if>

    <xsl:if test="$type = 2">
      <xsl:call-template name="classement_epreuve">
        <xsl:with-param name="epreuve" select="."/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <xsl:template name="se_prepare_epreuve">
    <xsl:param name="epreuve" />
    <div class="link">
      <a>
        <xsl:attribute name="href">
          <!--<xsl:value-of select="$site_url"/>-->
          <xsl:text>..</xsl:text>
          <!--<xsl:text>site/</xsl:text>-->
          <xsl:value-of select="@directory"/>
          <!--<xsl:value-of select="concat(translate(ancestor::competition[1]/titre, ' ', '_'), '_', translate(@nom_cateage, ' ', '_') ,'_', translate(@nom_catepoids, ' ', '_'), '_' ,translate(@sexe, ' ', '_'))"/>-->
          <xsl:text>/feuille_combats</xsl:text>
          <xsl:text>.html</xsl:text>
        </xsl:attribute>
        <xsl:value-of select="$epreuve/@libelle"/>
        <xsl:value-of select="$epreuve/@nom"/>
        <!--<xsl:value-of select="$epreuve/@nom_catepoids"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@sexe"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@nom_cateage"/>-->
      </a>
    </div>
  </xsl:template>

  <xsl:template name="avancement_epreuve">
    <xsl:param name="epreuve" />

    <xsl:variable select="number($epreuve/@typePhase)" name="type1"/>
    <xsl:variable select="number($epreuve/@typePhase)" name="type2"/>

    <xsl:if test="count($epreuve/phases/phase[number(@typePhase) = 1]) > 0">
      <div class="link">
        <a>
          <xsl:attribute name="href">
            <!--<xsl:value-of select="$site_url"/>-->
            <xsl:text>..</xsl:text>
            <!--<xsl:text>site/</xsl:text>-->
            <xsl:value-of select="@directory"/>
            <!--<xsl:value-of select="concat(translate(ancestor::competition[1]/titre, ' ', '_'), '_', translate(@nom_cateage, ' ', '_') ,'_', translate(@nom_catepoids, ' ', '_'), '_' ,translate(@sexe, ' ', '_'))"/>-->
            <xsl:text>/poules_resultats</xsl:text>
            <!--<xsl:value-of select="ancestor::competition[1]/titre"/>-->
            <xsl:text>.html</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="$epreuve/@libelle"/>
          <xsl:value-of select="$epreuve/@nom"/>
          <!--<xsl:value-of select="$epreuve/@nom_catepoids"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@sexe"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@nom_cateage"/>-->
          <xsl:text>&#32;Poules</xsl:text>
        </a>
      </div>
    </xsl:if>
    <xsl:if test="count($epreuve/phases/phase[number(@typePhase) = 2]) > 0">
      <div class="link">
        <a>
          <xsl:attribute name="href">
            <!--<xsl:value-of select="$site_url"/>-->
            <xsl:text>..</xsl:text>
            <!--<xsl:text>site/</xsl:text>-->
            <xsl:value-of select="@directory"/>
            <!--<xsl:value-of select="concat(translate(ancestor::competition[1]/titre, ' ', '_'), '_', translate(@nom_cateage, ' ', '_') ,'_', translate(@nom_catepoids, ' ', '_'), '_' ,translate(@sexe, ' ', '_'))"/>-->
            <xsl:text>/tableau_competition</xsl:text>
            <!--<xsl:value-of select="ancestor::competition[1]/titre"/>-->
            <xsl:text>.html</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="$epreuve/@libelle"/>
          <xsl:value-of select="$epreuve/@nom"/>
          <!--<xsl:value-of select="$epreuve/@nom_catepoids"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@sexe"/>
          <xsl:text>&#32;</xsl:text>
          <xsl:value-of select="$epreuve/@nom_cateage"/>-->
          <xsl:text>&#32;Tableau</xsl:text>
        </a>
      </div>

    </xsl:if>
  </xsl:template>

  <xsl:template name="classement_epreuve">
    <xsl:param name="epreuve" />
    <div class="link">
      <a>
        <xsl:attribute name="href">
          <!--<xsl:value-of select="$site_url"/>-->
          <xsl:text>..</xsl:text>
          <!--<xsl:text>site/</xsl:text>-->
          <xsl:value-of select="@directory"/>
          <!--<xsl:value-of select="concat(translate(ancestor::competition[1]/titre, ' ', '_'), '_', translate(@nom_cateage, ' ', '_') ,'_', translate(@nom_catepoids, ' ', '_'), '_' ,translate(@sexe, ' ', '_'))"/>-->
          <xsl:text>/classement_final</xsl:text>
          <xsl:text>.html</xsl:text>
        </xsl:attribute>
        <xsl:value-of select="$epreuve/@libelle"/>
        <xsl:value-of select="$epreuve/@nom"/>
        <!--<xsl:value-of select="$epreuve/@nom_catepoids"/>
        <xsl:text>&#32;</xsl:text>
        <xsl:value-of select="$epreuve/@sexe"/>
        <xsl:text>&#32;</xsl:text>
        <xsl:value-of select="$epreuve/@nom_cateage"/>-->
      </a>
    </div>
  </xsl:template>

</xsl:stylesheet>