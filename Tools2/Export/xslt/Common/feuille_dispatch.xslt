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
          file:///<xsl:value-of select="$style"/>style_dispatch.css
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
          <!--<xsl:if test="position() != 1" >
            <xsl:attribute name="style">
              page-break-before : always;
            </xsl:attribute>
          </xsl:if>-->

          <div class="poule_title">
            <div id='poule_title_div'>
              <xsl:text disable-output-escaping='yes'>Tapis </xsl:text>
              <xsl:value-of select="$tapis"/>
            </div>
          </div>

          <xsl:for-each select="./groupe">
            <xsl:variable name="groupe" select="@groupe_id" />
            <xsl:variable name="phase" select="@phase_id" />

            <xsl:variable name="epreuve_nom" select="@epreuve_nom" />
            <xsl:variable name="phase_nom" select="@phase_libelle" />

            <xsl:if test="count(//combat[@groupe = $groupe and ancestor::tapis[1]/@tapis = $tapis] ) &gt; 0">

              <!--<xsl:if test="@phase_type = 2">-->
                <div class="val">
                  <xsl:value-of select="$epreuve_nom"/>
                  <xsl:text disable-output-escaping="yes"> - </xsl:text>
                  <xsl:value-of select="$phase_nom"/>
                  <xsl:text disable-output-escaping="yes"> - </xsl:text>
                  <xsl:value-of select="@groupe_libelle"/>
                </div>
              <!--</xsl:if>-->

              <!--<xsl:if test="@phase_type = 1">
                <div class="val">

                  <xsl:for-each select="//poule[@phase = $phase and ancestor::tapis[1]/@tapis = $tapis]">
                    <xsl:sort select="@numero" data-type="number" order="ascending"/>
                    <div class="val">
                      <xsl:value-of select="$epreuve_nom"/>
                      <xsl:text disable-output-escaping="yes"> - </xsl:text>
                      <xsl:value-of select="@nom"/>
                    </div>
                  </xsl:for-each>

              
                </div>
              </xsl:if>-->

            </xsl:if>

          </xsl:for-each>

        </div>
      </xsl:for-each>
    </body>
  </xsl:template>
</xsl:stylesheet>