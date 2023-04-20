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
    <html style="overflow:hidden;height: 100%;">
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
      <link type="text/css" rel="stylesheet" href="../style/style_iframe1.css" ></link>

      <!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>

      <!--<xsl:variable name="css1">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_menu.css</xsl:text>
      </xsl:variable>
      <xsl:variable name="css2">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_iframe1.css</xsl:text>
      </xsl:variable>

      <style type="text/css">
        <xsl:value-of select="document($css1)" disable-output-escaping="yes" />
        <xsl:value-of select="document($css2)" disable-output-escaping="yes" />
      </style>

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>-->

      <title>
        <xsl:value-of select="@titre"/>
      </title>
    </head>
    <body style="height: 100%;">
    
      <div class="btn_menu">
        <a class="btn btn-warning" href="../common/menu.html" role="button">
			<img class="img" src="../img/home-32.png" alt="Menu"/>
		</a>
      </div>

      <div class="panel panel-primary">
        <div class="panel-heading clearfix" style="text-align:center;">
          
                <span style="font-size:1.0em;">
                  <xsl:value-of select="//competition[1]/titre"/>
                  <xsl:text> - </xsl:text>
                  <xsl:value-of select="//competition[1]/lieu"/>
                </span>
        
        </div>

        <div class="panel-body">
          <div class="video">
            <div class="embed-container">
              <iframe src="../common/tapis_All0.html" frameborder="0" allowfullscreen=""></iframe>
            </div>
          </div>
        </div>
      </div>

    </body>
  </xsl:template>

</xsl:stylesheet>