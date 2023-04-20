<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema">

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"></xsl:param>
	<xsl:param name="js"></xsl:param>

	<xsl:param name="istapis"></xsl:param>

	<xsl:variable name="couleur1" select="//competition/@couleur1">
	</xsl:variable>
	<xsl:variable name="couleur2" select="//competition/@couleur2">
	</xsl:variable>

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
			<link type="text/css" rel="stylesheet" href="../style/style_combats.css" ></link>
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

			<xsl:variable name="epreuve" select="//epreuve[1]/@ID"></xsl:variable>

			<div class="btn_defilement">

				<xsl:if test="$istapis = 'tapis' and count(//tapis) = 1">
					<div class="panel-body" style="padding:0px;">
						<xsl:variable name="tapis" select="//tapis[1]/@tapis" />
						<div class="panel panel-info">
							<div class="panel-heading clearfix">
								<span style="width: 100%;font-weight: 600;">
									Tapis <xsl:value-of select="$tapis"/>
								</span>

								<div style="position: fixed;right: 10px;top: 10px;">
									<a class="btn btn-danger" onclick="anim2();" style="margin-right: 10px;">
										<img class="img" src="../img/refresh-32.png" alt="Actualiser"/>
									</a>
									<a class="btn btn-info" onclick="setDefilement();">
										<img class="img" src="../img/repeat-32.png" alt="Défilement"/>
									</a>
								</div>

								<!--<a class="btn btn-danger" onclick="anim2();"  style="margin-right: 10px;">Actualiser</a>
                    <a class="btn btn-info" onclick="setDefilement();">Défilement</a>-->
							</div>
						</div>
					</div>
				</xsl:if>

				<xsl:if test="$istapis != 'tapis' or count(//tapis) != 1">
					<div style="position: fixed;right: 10px;top: 10px;">
						<a class="btn btn-danger" onclick="anim2();"  style="margin-right: 10px;">
							<img class="img" src="../img/refresh-32.png" alt="Actualiser"/>
						</a>
						<a class="btn btn-info" onclick="setDefilement();">
							<img class="img" src="../img/repeat-32.png" alt="Défilement"/>
						</a>
					</div>

				</xsl:if>
			</div>


			<xsl:if test="$istapis != 'tapis'">

				<div class="btn_menu">
					<a class="btn btn-warning" href="../common/menu.html" role="button">
						<img class="img" src="../img/home-32.png" alt="Menu"/>
					</a>
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
							<xsl:value-of select="//epreuve[1]/@nom"/>
						</span>

					</div>
				</div>

			</xsl:if>



			<div class="panel-body" style="padding:0px;">
				<xsl:for-each select="//tapis">
					<xsl:sort select="@tapis" data-type="number" order="ascending"/>

					<xsl:if test="@tapis != 0  and ($istapis != 'epreuve' or count(./combats/combat) &gt; 0)">
						<xsl:variable name="tapis" select="@tapis" />

						<div class="panel panel-info">

							<div class="panel-heading clearfix">
								<span style="width: 100%;font-weight: 600;">
									Tapis <xsl:value-of select="$tapis"/>
								</span>
							</div>
							<div class="panel-body" style="padding:0px;">
								<xsl:for-each select="//tapis[@tapis=$tapis]/combats/combat">
									<xsl:sort select="@time_programmation" data-type="number" order="ascending"/>
									
									<xsl:if test="$istapis = 'epreuve' or position() &lt; 6">
										<xsl:call-template name="combat" >
											<xsl:with-param name="combat" select="."/>
										</xsl:call-template>
									</xsl:if>
								</xsl:for-each>

							</div>
						</div>
					</xsl:if>
				</xsl:for-each>

			</div>

		</body>
	</xsl:template>


	<xsl:template name="combat">
		<xsl:param name="combat"/>
		<xsl:param name="img"/>

		<xsl:variable name="epreuve" select="$combat/@epreuve" />
		<xsl:variable name="phase" select="$combat/@phase" />

		<xsl:variable name="participant1" select="$combat/score[1]/@judoka"/>
		<xsl:variable name="judoka1" select="//participant[@judoka=$participant1]/descendant::*[1]"/>
		<xsl:variable name="club1" select="$judoka1/@club"/>
		<xsl:variable name="comite1" select="//club[@ID=$club1]/@comite"/>
		<xsl:variable name="ligue1" select="//club[@ID=$club1]/@ligue"/>

		<xsl:variable name="participant2" select="$combat/score[2]/@judoka"/>
		<xsl:variable name="judoka2" select="//participant[@judoka=$participant2]/descendant::*[1]"/>
		<xsl:variable name="club2" select="$judoka2/@club"/>
		<xsl:variable name="comite2" select="//club[@ID=$club2]/@comite"/>
		<xsl:variable name="ligue2" select="//club[@ID=$club2]/@ligue"/>

		<div class="row bs-callout bs-callout-info">
			<div class="col-xs-5">
				<a href="#" role="button">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test= "$participant1 = 'null'">
								<xsl:text disable-output-escaping="yes">btn btn-secondary btn-lg combatname</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
								<xsl:choose>
									<xsl:when test= "$couleur1 = 'Bleu'">
										<xsl:text disable-output-escaping="yes">btn btn-primary btn-lg combatname</xsl:text>
									</xsl:when>
									<xsl:when test= "$couleur1 = 'Rouge'">
										<xsl:text disable-output-escaping="yes">btn btn-danger btn-lg combatname</xsl:text>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">btn btn-default btn-lg combatname</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>

					</xsl:attribute>

					<!-- Affiche le nom du Judoka s'il n'est pas null, "En Attente" sinon -->
					<xsl:choose>
						<xsl:when test= "$participant1 = 'null'">
							<!-- Combat en attente-->
							<span class="center">
								<img class="img" src="../img/sablier.png"/>
								<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
							</span>
						</xsl:when>
						<xsl:otherwise>
							<span class="right">
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka=$participant1]/descendant::*[1]/@nom"/>
								<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka=$participant1]/descendant::*[1]/@prenom"/>
							</span>
							<span class="left club">
								<xsl:variable name="ecartement1" select="//phase[@id=$phase]/@ecartement"/>
								<xsl:choose>
									<xsl:when test= "$ecartement1 = '3'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite1"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite1"/>
										</xsl:if>

									</xsl:when>

									<xsl:when test= "$ecartement1 = '4'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="//ligue[@ID=$ligue1]/nomCourt"/>
										</xsl:if>
									</xsl:when>

									<xsl:otherwise>
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club1]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite1"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite1"/>
										</xsl:if>
									</xsl:otherwise>
								</xsl:choose>
							</span>
						</xsl:otherwise>
					</xsl:choose>
				</a>
			</div>

			<div class="col-xs-2">
				<a class="btn btn-success btn-lg combatname" href="#" role="button">
					<span class="">
						<!--<xsl:value-of select="//epreuve[@ID=$epreuve]/@remoteId_cateage"/>
            <xsl:text disable-output-escaping="yes">&#032;</xsl:text>-->
						<xsl:if test="$typeCompetition != 1">
							<xsl:value-of select="//epreuve[@ID=$epreuve]/@nom"/>
						</xsl:if>
						<!--<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
            <xsl:value-of select="//epreuve[@ID=$epreuve]/@sexe"/>-->

						<xsl:if test="$typeCompetition = 1">
							<xsl:value-of select="//epreuve[@ID=$epreuve]/@nom"/>
							<!--<xsl:value-of select="/h:html/h:head/h:title"/>-->
							<!--<xsl:if test="//a[@href = 'htt://192.168.0.24:8080/site/37160/common/tapis_All4.html']">
				       <xsl:if test="//epreuve[@ID=$epreuve]/@nom = 'Senior Feminin'">
					      <xsl:text disable-output-escaping="yes">S&#032;F</xsl:text>			  
				       </xsl:if>
			  	       <xsl:if test="//epreuve[@ID=$epreuve]/@nom = 'Senior Masculin'">
					      <xsl:text disable-output-escaping="yes">S&#032;M</xsl:text>			  
				       </xsl:if>
				  </xsl:if>-->
						</xsl:if>
					</span>
				</a>
			</div>

			<div class="col-xs-5">
				<a class="" href="#" role="button">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test= "$participant2 = 'null'">
								<xsl:text disable-output-escaping="yes">btn btn-secondary btn-lg combatname</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<!-- Le participant n'est pas null on peut prendre en compte la couleur de ceinture -->
								<xsl:choose>
									<xsl:when test= "$couleur2 = 'Bleu'">
										<xsl:text disable-output-escaping="yes">btn btn-primary btn-lg combatname</xsl:text>
									</xsl:when>

									<xsl:when test= "$couleur2 = 'Rouge'">
										<xsl:text disable-output-escaping="yes">btn btn-danger btn-lg combatname</xsl:text>
									</xsl:when>

									<xsl:otherwise>
										<xsl:text disable-output-escaping="yes">btn btn-default btn-lg combatname</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>

					</xsl:attribute>
					<!-- Ici c'est le nom du 2nd Judoka -->
					<xsl:choose>
						<xsl:when test= "$participant2 = 'null'">
							<!-- Combat en attente-->
							<span class="center">
								<img class="img" src="../img/sablier.png"/>
								<xsl:text disable-output-escaping="yes">&#032;En Attente</xsl:text>
							</span>
						</xsl:when>
						<xsl:otherwise>
							<span class="left">
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka=$participant2]/descendant::*[1]/@nom"/>
								<xsl:text disable-output-escaping="yes">&#032;</xsl:text>
								<xsl:value-of select="ancestor::tapis[1]/participants/participant[@judoka=$participant2]/descendant::*[1]/@prenom"/>
							</span>

							<span class="right club" >
								<xsl:variable name="ecartement2" select="//phase[@id=$phase]/@ecartement"/>
								<xsl:choose>
									<xsl:when test= "$ecartement2 = '3'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite2"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite2"/>
										</xsl:if>
									</xsl:when>

									<xsl:when test= "$ecartement2 = '4'">
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="//ligue[@ID=$ligue2]/nomCourt"/>
										</xsl:if>
									</xsl:when>

									<xsl:otherwise>
										<xsl:if test="$typeCompetition != '1'">
											<xsl:value-of select="//club[@ID=$club2]/nomCourt"/>
											<xsl:text disable-output-escaping="yes">&#032;-&#032;</xsl:text>
											<xsl:value-of select="$comite2"/>
										</xsl:if>
										<xsl:if test="$typeCompetition = '1'">
											<xsl:value-of select="$comite2"/>
										</xsl:if>
									</xsl:otherwise>
								</xsl:choose>
							</span>
						</xsl:otherwise>
					</xsl:choose>
				</a>
			</div>

		</div>
	</xsl:template>
</xsl:stylesheet>