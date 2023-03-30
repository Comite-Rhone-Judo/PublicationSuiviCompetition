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
      <meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no" />
      <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
      <meta http-equiv="Pragma" content="no-cache" />
      <meta http-equiv="Expires" content="0" />
      <link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

      <!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>
      
      

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body>

      <!--<div class="col-md-12">
        <div class="panel-heading">
          <nav class="navbar navbar-default" style="margin-bottom: 0;">
            <div class="container-fluid">
              <div class="navbar-header">
                <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                  <span class="sr-only">Toggle navigation</span>
                  <span class="icon-bar"></span>
                  <span class="icon-bar"></span>
                  <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="#" id="LabelTitre" >
                 
                </a>
              </div>
            </div>
          </nav>
        </div>
      </div>-->

      <!--<div class="col-md-12">-->
        <div class="panel panel-default">
          <!--<div class="panel-heading clearfix">
            <span id="ContentPlaceHolder1_Label2" style="width: 100%;">
              <xsl:value-of select="//competition[1]/titre"/>
              <xsl:text> - </xsl:text>
              <xsl:value-of select="//competition[1]/lieu"/>
            </span>
          </div>-->

          <div class="panel-body">

            <div class="col-md-12" style="margin-top: 15px;">
              <ul class="nav nav-pills nav-justified" role="tablist" style="margin-bottom: 20px;">
                <li id="tab1" class="active">
                  <a onclick="set_tab(1, 'form');">Se prépare</a>
                </li>
                <li id="tab2" class="">
                  <a onclick="set_tab(2, 'form');">Prochains combats</a>
                </li>
                <li id="tab3" class="">
                  <a onclick="set_tab(3, 'form');">Avancements</a>
                </li>
                <li id="tab4" class="">
                  <a onclick="set_tab(4, 'form');">Classements</a>
                </li>
              </ul>
            </div>

            <div id="div1" class="col-md-12">
              <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
                <a href="../common/tapis_All.html" class="alert-link">
                  Affichage tous les tapis
                </a>
              </div>
              <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
                <a href="../common/tapis_All1.html" class="alert-link">
                  Affichage 1 Tapis
                </a>
              </div>
              <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
                <a href="../common/tapis_All2.html" class="alert-link">
                  Affichage 2 Tapis
                </a>
              </div>
              <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
                <a href="../common/tapis_All4.html" class="alert-link">
                  Affichage 4 Tapis
                </a>
              </div>
              <!--<div class="link">
                <a href="../common/tapis_All4.html">
                  Affichage 4 Tapis
                </a>
              </div>-->
            </div>

            <div id="div2" class="col-md-12">
              <xsl:for-each select="/competitions/competition">
                <xsl:if test="count(./epreuve) > 0">
                  <div class="col-md-12">
                    <div class="panel panel-info" style="text-align: center;">
                      <div class="panel-heading clearfix">
                        <span id="ContentPlaceHolder1_Label2" style="width: 100%;">
                          <xsl:value-of select="./titre"/>
                        </span>
                      </div>
                      <div class="panel-body">
                        <xsl:if test="count(./epreuve[@sexe='F']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories féminines
                            </div>

                            <xsl:apply-templates select="./epreuve[@sexe='F']">
                              <xsl:with-param name="type">
                                <xsl:number value="0"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[@sexe='M']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories masculines
                            </div>
                            <xsl:apply-templates select="./epreuve[@sexe='M']">
                              <xsl:with-param name="type">
                                <xsl:number value="0"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[not(@sexe)]) > 0">
                          <div class="row">                           
                            <xsl:apply-templates select="./epreuve[not(@sexe)]">
                              <xsl:with-param name="type">
                                <xsl:number value="0"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>
                        
                      </div>
                    </div>
                  </div>
                </xsl:if>
              </xsl:for-each>

            </div>

            <div id="div3" class="col-md-12">
              <xsl:for-each select="/competitions/competition">
                <xsl:if test="count(./epreuve) > 0">

                  <div class="col-md-12">
                    <div class="panel panel-info" style="text-align: center;">
                      <div class="panel-heading clearfix">
                        <span id="ContentPlaceHolder1_Label2" style="width: 100%;">
                          <xsl:value-of select="./titre"/>
                        </span>
                      </div>

                      <div class="panel-body">
                        <xsl:if test="count(./epreuve[@sexe='F']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories féminines
                            </div>
                            <xsl:apply-templates select="./epreuve[@sexe='F']">
                              <xsl:with-param name="type">
                                <xsl:number value="1"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[@sexe='M']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories masculines
                            </div>
                            <xsl:apply-templates select="./epreuve[@sexe='M']">
                              <xsl:with-param name="type">
                                <xsl:number value="1"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[not(@sexe)]) > 0">
                          <div class="row">
                            <xsl:apply-templates select="./epreuve[not(@sexe)]">
                              <xsl:with-param name="type">
                                <xsl:number value="1"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>
                      </div>
                    </div>
                  </div>
                </xsl:if>
              </xsl:for-each>
            </div>

            <div id="div4" class="col-md-12">
              <xsl:for-each select="/competitions/competition">
                <xsl:if test="count(./epreuve) > 0">
                  <div class="col-md-12">
                    <div class="panel panel-info" style="text-align: center;">
                      <div class="panel-heading clearfix">
                        <span id="ContentPlaceHolder1_Label2" style="width: 100%;">
                          <xsl:value-of select="./titre"/>
                        </span>
                      </div>

                      <div class="panel-body">
                        <xsl:if test="count(./epreuve[@sexe='F']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories féminines
                            </div>

                            <xsl:apply-templates select="./epreuve[@sexe='F']">
                              <xsl:with-param name="type">
                                <xsl:number value="2"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[@sexe='M']) > 0">
                          <div class="row">
                            <div class="alert alert-success" role="alert" style="padding: 5px; text-align:left;">
                              Catégories masculines
                            </div>
                            <xsl:apply-templates select="./epreuve[@sexe='M']">
                              <xsl:with-param name="type">
                                <xsl:number value="2"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>

                        <xsl:if test="count(./epreuve[not(@sexe)]) > 0">
                          <div class="row">
                            <xsl:apply-templates select="./epreuve[not(@sexe)]">
                              <xsl:with-param name="type">
                                <xsl:number value="2"/>
                              </xsl:with-param>
                            </xsl:apply-templates>
                          </div>
                        </xsl:if>
                        
                      </div>
                    </div>
                  </div>
                </xsl:if>
              </xsl:for-each>

            </div>

          </div>
        </div>
      <!--</div>-->
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
    <div class="col-lg-6">
      <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
        <a class="alert-link">
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
    </div>
  </xsl:template>

  <xsl:template name="avancement_epreuve">
    <xsl:param name="epreuve" />

    <xsl:variable select="number($epreuve/@typePhase)" name="type1"/>
    <xsl:variable select="number($epreuve/@typePhase)" name="type2"/>

    <xsl:if test="count($epreuve/phases/phase[number(@typePhase) = 1]) > 0">
      <div class="col-lg-6">
        <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
          <a class="alert-link">
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
      </div>
    </xsl:if>
    <xsl:if test="count($epreuve/phases/phase[number(@typePhase) = 2]) > 0">
      <div class="col-lg-6">
        <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
          <a class="alert-link">
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
      </div>

    </xsl:if>
  </xsl:template>

  <xsl:template name="classement_epreuve">
    <xsl:param name="epreuve" />
    <div class="col-lg-6">
      <div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
        <a class="alert-link">
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
    </div>
  </xsl:template>

</xsl:stylesheet>