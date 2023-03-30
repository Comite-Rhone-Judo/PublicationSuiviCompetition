<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<!--<xsl:param name="menu"></xsl:param>-->
	<xsl:param name="js"></xsl:param>

	<xsl:key name="participants" match="participant" use="@poule"/>

	<xsl:variable name="typeCompetition" select="/competition/@type">
	</xsl:variable>

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
			<link type="text/css" rel="stylesheet" href="../style/style_feuille_competition.css" ></link>
			<link type="text/css" rel="stylesheet" href="../style/style_menu.css" ></link>

			<!--<script src="../js/jquery.min.js"></script>
      <script src="../js/script.js"></script>-->

			<script type="text/javascript" >
				<xsl:value-of select="$js"/>
			</script>


			<!--<xsl:variable name="css1">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_feuille_competition.css</xsl:text>
      </xsl:variable>
      <xsl:variable name="css2">
        file:///<xsl:value-of select="$style"/>
        <xsl:text>style_menu.css</xsl:text>
      </xsl:variable>

      <style type="text/css">
        <xsl:value-of select="document($css1)" disable-output-escaping="yes" />
        <xsl:value-of select="document($css2)" disable-output-escaping="yes" />
      </style>

      <script type="text/javascript" >
        <xsl:value-of select="$js"/>
      </script>-->

			<title>
				<xsl:value-of select="//epreuve[1]/@nom"/>
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
					<xsl:apply-templates select="//phase/poules/poule"/>
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
          <xsl:text disable-output-escaping="yes">&#032;</xsl:text>
          <xsl:value-of select="//epreuve[1]/@nom"/>
          <xsl:value-of select="//epreuve[1]/@sexe"/>
        </div>
      </div>
      
      <div class="main">
        <xsl:apply-templates select="//phase/poules/poule"/>
      </div>-->
		</body>
	</xsl:template>

	<xsl:template match="poule">
		<xsl:variable name="numero" select="@numero"/>

		<div class="col-md-12" style="padding:0px;">
			<div class="panel panel-info" style="text-align:center;">

				<div class="panel-heading clearfix">
					<span style="width: 100%;font-weight: 600;">
						<xsl:value-of select="@nom"/>
					</span>
				</div>

				<div class="panel-body">

					<table class="t2">
						<tr>
							<th class="name">
								NOM et Prénom
							</th>
							<th class="name">
								Club
							</th>
							<th></th>
							<xsl:for-each select="key('participants',$numero)">
								<th class="align_center">
									<div class="alert alert-warning" role="alert">
										<xsl:value-of select="position()"/>
									</div>
								</th>
							</xsl:for-each>
							<th>V</th>
							<th>P</th>
						</tr>
						<xsl:apply-templates select="key('participants',$numero)">
							<xsl:sort select="@position" data-type="number" order="ascending"/>
							<xsl:with-param name="poule" select="$numero" />
						</xsl:apply-templates>
					</table>

				</div>
			</div>
		</div>


		<!--<div class="poule" style="page-break-inside : avoid;">
      <div>

        <div class="header3" style="text-align:center;" >
          <xsl:value-of select="@nom"/>
        </div>

        <div class="table_poule">
          <table class="t2">
            <tr>
              <th class="name">
                NOM et Prénom
              </th>
              <th class="name">
                Club
              </th>
              <th></th>
              <xsl:for-each select="key('participants',$numero)">
                <th class="align_center">
                  <xsl:value-of select="position()"/>
                </th>
              </xsl:for-each>
              <th>V</th>
              <th>P</th>
            </tr>
            <xsl:apply-templates select="key('participants',$numero)">
              <xsl:sort select="@position" data-type="number" order="ascending"/>
              <xsl:with-param name="poule" select="$numero" />
            </xsl:apply-templates>
          </table>
        </div>


        <div style="clear: both;"></div>
        <div style="clear: both;"></div>

      </div>
    </div>-->
	</xsl:template>


	<xsl:template match="participant">
		<xsl:param name="poule" />

		<xsl:variable name="participant1" select="@judoka" />
		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />
		<xsl:variable name="grade" select="$j1/@grade"/>


		<tr>
			<td class="align_left">
				<div class="alert alert-info" role="alert">
					<xsl:value-of select="$j1/@nom"/>
					<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
					<xsl:value-of select="$j1/@prenom"/>
				</div>
			</td>
			<td class="align_left">
				<div class="alert alert-info" role="alert">
					<xsl:variable name="club" select="$j1/@club"/>
					<xsl:variable name="clubN" select="//club[@ID=$club]"/>
					<xsl:variable name="comite" select="$clubN/@comite"/>
					<xsl:variable name="ligue" select="$clubN/@ligue"/>


					<xsl:variable name="ecartement" select="ancestor::phase[1]/@ecartement"/>
					<xsl:choose>
						<xsl:when test= "$ecartement = '3'">
							<xsl:value-of select="$clubN/nomCourt"/> - <xsl:value-of select="$comite"/>
						</xsl:when>

						<xsl:when test= "$ecartement = '4'">
							<xsl:value-of select="$clubN/nomCourt"/> - <xsl:value-of select="//ligue[@ID=$ligue]/nomCourt"/>
						</xsl:when>

						<xsl:otherwise>
							<xsl:value-of select="$clubN/nomCourt"/>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
			<td class="align_center">
				<div class="alert alert-warning" role="alert">
					<xsl:value-of select="position()"/>
				</div>
			</td>

			<xsl:for-each select="key('participants',$poule)">
				<xsl:sort select="@position" data-type="number" order="ascending"/>

				<xsl:variable name="participant2" select="@judoka"/>
				<xsl:if test= "$participant1=$participant2">
					<td class="tcombatsilver"></td>
				</xsl:if>
				<xsl:if test= "$participant1!=$participant2">
					<td class="tcombat">
						<xsl:apply-templates select="//combat[(score[1][@judoka = $participant1] and score[2][@judoka = $participant2]) or (score[2][@judoka = $participant1] and score[1][@judoka = $participant2])]">
							<xsl:with-param name="participant1" select="$participant1" />
						</xsl:apply-templates>
					</td>
				</xsl:if>
			</xsl:for-each>
			<td class="align_center">
				<xsl:value-of select="@nbVictoires"/>
			</td>
			<td class="align_center">
				<xsl:value-of select="@cumulPoints"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="combat[not(@vainqueur) or @vainqueur= '-1']">

		<table class="tscore">
			<tr>
				<td>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="combat[@vainqueur and @vainqueur != '-1']">
		<xsl:param name="participant1" />

		<xsl:variable name="participant2">
			<xsl:if test= "$participant1 != ./score[1]/@judoka">
				<xsl:value-of select="./score[1]/@judoka"/>
			</xsl:if>
			<xsl:if test= "$participant1 != ./score[2]/@judoka">
				<xsl:value-of select="./score[2]/@judoka"/>
			</xsl:if>
		</xsl:variable>
		<div class="alert alert-success" role="alert" style="padding:5px 0px;">

			<xsl:if test= "$participant1!=@vainqueur">
				<xsl:attribute name="class">
					alert alert-danger
				</xsl:attribute>
			</xsl:if>

			<xsl:if test= "$participant1=@vainqueur">
				<xsl:attribute name="class">
					alert alert-success
				</xsl:attribute>
			</xsl:if>

			<table class="tscore">
				<tr>
					<td>
						<xsl:if test= "$participant1!=@vainqueur">
							<xsl:attribute name="class">
								tcombatperdant
							</xsl:attribute>
							<xsl:text>X</xsl:text>
						</xsl:if>

						<xsl:if test= "$participant1=@vainqueur">
							<xsl:attribute name="class">
								name perdant
							</xsl:attribute>
						</xsl:if>

						<xsl:if test= "$participant1=@vainqueur">
							<span>
								<xsl:value-of select="substring(./@scorevainqueur,0,3)"/>
							</span>

							<xsl:if test="$typeCompetition != '1'">
								<span style="color:red;">
									<xsl:value-of select="./@penvainqueur"/>
								</span>
							</xsl:if>
						</xsl:if>
					</td>
				</tr>
				<xsl:if test= "$participant1=@vainqueur">
					<tr>
						<td colspan="2" class="name">
							<span>
								<xsl:value-of select="substring(./@scoreperdant,0,3)"/>
							</span>
							<xsl:if test="$typeCompetition != '1'">
								<span style="color:red;">
									<xsl:value-of select="./@penperdant"/>
								</span>
							</xsl:if>

						</td>
					</tr>
				</xsl:if>
			</table>
		</div>
	</xsl:template>
</xsl:stylesheet>