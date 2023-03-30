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
          file:///<xsl:value-of select="$style"/>header.css
        </xsl:attribute>
      </link>

      <title>
        <xsl:value-of select="//epreuve[1]/@nom"/>
      </title>




    </head>
    <body>

      <div class="main">

        <xsl:if test="//image[@type=1]/@image" >
          <div class="col1">
            <img border="0" align="center" height="100px">
              <xsl:attribute name="src">
                <xsl:value-of select="//image[@type=1]/@image"/>
              </xsl:attribute>
              <xsl:text>&#32;</xsl:text>
            </img>
          </div>
        </xsl:if>
        <xsl:if test="//image[@type=2]/@image" >
          <div class="col3">
            <img border="0" align="center" height="100px">
              <xsl:attribute name="src">
                <xsl:value-of select="//image[@type=2]/@image"/>
              </xsl:attribute>
              <xsl:text>&#32;</xsl:text>
            </img>
          </div>
        </xsl:if>
        <div class="col2" style="height:100px;">
          <span>
            <xsl:value-of select="./titre"/>
            <xsl:text>&#32;-&#32;</xsl:text>
            <xsl:value-of select="./lieu"/>
            <br/>
            <xsl:text>POULES</xsl:text>
            <xsl:text>&#32;(Le</xsl:text>

            <xsl:variable name="date" select="./@date">
            </xsl:variable>

            <xsl:value-of select="concat(substring(@date, 1, 2), '-', substring(@date, 3, 2), '-',substring(@date, 5))" />
            <xsl:text>)</xsl:text>



            <!--<xsl:value-of select="./titre"/>
            <xsl:text> Le </xsl:text>
            
            <xsl:variable name="date" select="./@date">
            </xsl:variable>            

            <xsl:value-of select="concat(substring(@date, 1, 2), '-', substring(@date, 3, 2), '-',substring(@date, 5))" />
            <br/>
            <xsl:text>POULES</xsl:text>-->
            
            
            <!--<xsl:value-of select="//epreuve[1]/@nom_cateage"/>
            <xsl:text>&#32;</xsl:text>
            <xsl:value-of select="//epreuve[1]/@sexe"/>
            <xsl:text>&#32;</xsl:text>
            <xsl:value-of select="//epreuve[1]/@nom_catepoids"/>
            <xsl:text>&#32;(</xsl:text>
            <xsl:value-of select="count(//participant)"/>
            <xsl:text>&#32;combattants)</xsl:text>-->
          </span>
        </div>
      </div>
    </body>
  </xsl:template>
</xsl:stylesheet>