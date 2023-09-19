<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>
	<xsl:template match="/">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<xsl:apply-templates/>
		</html>
	</xsl:template>


	<xsl:template match="/*">
		<!-- ENTETE HTML -->
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
			<meta name="viewport" content="width=device-width, initial-scale=1.0, shrink-to-fit=no"/>
			<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
			<meta http-equiv="Pragma" content="no-cache"/>
			<meta http-equiv="Expires" content="0"/>
			<link type="text/css" rel="stylesheet" href="../style/style_menu.css"/>
			<link type="text/css" rel="stylesheet" href="../style/style_iframe1.css"/>

			<script type="text/javascript">
				<xsl:value-of select="$js"/>
			</script>
			<title>
				<xsl:value-of select="@titre"/>
			</title>
		</head>
		<body>
			<div class="panel panel-primary" style="text-align: center;">
				<div class="panel-body">
					<div class="container">
						<!-- TITRE RHONE (Logo & entete) -->
						<div class="row">
							<div class="col-md-12">
								<div class="col-xs-6">
									<img class="img" src="../img/France-Judo-Rhone.png" width="100"
									/>
								</div>
								<div class="col-xs-6">Suivi compétition</div>
							</div>
						</div>
						<!-- CONTENU de la page -->
						<!-- BOUTONS de selection -->
						<div class="row">
							<div class="col-md-12" style="margin-top: 15px;">
								<ul class="nav nav-pills nav-justified" role="tablist"
									style="margin-bottom: 20px;">
									<xsl:if test="/competitions/@PublierProchainsCombats = 'true'">
										<li id="tab1" class="active">
											<a onclick="set_tab(1, 'form');">Se prépare</a>
										</li>
										<li id="tab2" class="">
											<a onclick="set_tab(2, 'form');">Prochains combats</a>
										</li>
									</xsl:if>
									<li id="tab3" class="">
										<a onclick="set_tab(3, 'form');">Avancements</a>
									</li>
									<xsl:if test="/competitions/@PublierAffectationTapis = 'true'">
										<li id="tab5" class="">
											<a onclick="set_tab(5, 'form');">Affectation</a>
										</li>
									</xsl:if>
									<li id="tab4" class="">
										<a onclick="set_tab(4, 'form');">Classements</a>
									</li>
								</ul>
							</div>
						</div>

						<!-- VUE PAR TAPIS -->
						<xsl:if test="/competitions/@PublierProchainsCombats = 'true'">
							<div class="row">
								<div id="div1" class="col-md-12">
									<div class="alert alert-warning" role="alert"
										style="padding: 5px; text-align:left;">
										<a href="../common/tapis_All.html" class="alert-link">
											Affichage tous les tapis </a>
									</div>
									<div class="alert alert-warning" role="alert"
										style="padding: 5px; text-align:left;">
										<a href="../common/tapis_All1.html" class="alert-link">
											Affichage 1 Tapis </a>
									</div>
									<div class="alert alert-warning" role="alert"
										style="padding: 5px; text-align:left;">
										<a href="../common/tapis_All2.html" class="alert-link">
											Affichage 2 Tapis </a>
									</div>
									<div class="alert alert-warning" role="alert"
										style="padding: 5px; text-align:left;">
										<a href="../common/tapis_All4.html" class="alert-link">
											Affichage 4 Tapis </a>
									</div>
								</div>
							</div>
						</xsl:if>

						<!-- VUE SE PREPARE -->
						<xsl:if test="/competitions/@PublierProchainsCombats = 'true'">
							<div class="row">
								<div id="div2" class="col-md-12">
									<xsl:for-each select="/competitions/competition">
										<xsl:if test="count(./epreuve) > 0">
											<div class="col-md-12">
												<div class="panel panel-info"
												style="text-align: center;">
												<div class="panel-heading clearfix">
												<span id="ContentPlaceHolder1_Label2"
												style="width: 100%;">
												<xsl:value-of select="./titre"/>
												</span>
												</div>
												<div class="panel-body">
												<xsl:if test="count(./epreuve[@sexe = 'F']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												féminines </div>

												<xsl:apply-templates
												select="./epreuve[@sexe = 'F']">
												<xsl:with-param name="type">
												<xsl:number value="0"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[@sexe = 'M']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												masculines </div>
												<xsl:apply-templates
												select="./epreuve[@sexe = 'M']">
												<xsl:with-param name="type">
												<xsl:number value="0"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[not(@sexe)]) > 0">
												<div class="row">
												<xsl:apply-templates
												select="./epreuve[not(@sexe)]">
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
							</div>
						</xsl:if>

						<!-- VUE AVANCEMENT -->
						<div class="row">
							<div id="div3" class="col-md-12">
								<xsl:for-each select="/competitions/competition">
									<xsl:if test="count(./epreuve) > 0">

										<div class="col-md-12">
											<div class="panel panel-info"
												style="text-align: center;">
												<div class="panel-heading clearfix">
												<span id="ContentPlaceHolder1_Label2"
												style="width: 100%;">
												<xsl:value-of select="./titre"/>
												</span>
												</div>

												<div class="panel-body">
												<xsl:if test="count(./epreuve[@sexe = 'F']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												féminines </div>
												<xsl:apply-templates
												select="./epreuve[@sexe = 'F']">
												<xsl:with-param name="type">
												<xsl:number value="1"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[@sexe = 'M']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												masculines </div>
												<xsl:apply-templates
												select="./epreuve[@sexe = 'M']">
												<xsl:with-param name="type">
												<xsl:number value="1"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[not(@sexe)]) > 0">
												<div class="row">
												<xsl:apply-templates
												select="./epreuve[not(@sexe)]">
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
						</div>

						<!-- VUE CLASSEMENT -->
						<div class="row">
							<div id="div4" class="col-md-12">
								<xsl:for-each select="/competitions/competition">
									<xsl:if test="count(./epreuve) > 0">
										<div class="col-md-12">
											<div class="panel panel-info"
												style="text-align: center;">
												<div class="panel-heading clearfix">
												<span id="ContentPlaceHolder1_Label2"
												style="width: 100%;">
												<xsl:value-of select="./titre"/>
												</span>
												</div>

												<div class="panel-body">
												<xsl:if test="count(./epreuve[@sexe = 'F']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												féminines </div>

												<xsl:apply-templates
												select="./epreuve[@sexe = 'F']">
												<xsl:with-param name="type">
												<xsl:number value="2"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[@sexe = 'M']) > 0">
												<div class="row">
												<div class="alert alert-success" role="alert"
												style="padding: 5px; text-align:left;"> Catégories
												masculines </div>
												<xsl:apply-templates
												select="./epreuve[@sexe = 'M']">
												<xsl:with-param name="type">
												<xsl:number value="2"/>
												</xsl:with-param>
												</xsl:apply-templates>
												</div>
												</xsl:if>

												<xsl:if test="count(./epreuve[not(@sexe)]) > 0">
												<div class="row">
												<xsl:apply-templates
												select="./epreuve[not(@sexe)]">
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

						<!-- VUE AFFECTATION -->
						<xsl:if test="/competitions/@PublierAffectationTapis = 'true'">
							<div class="row">
								<div id="div5" class="col-md-12"
									style="height:100%;width:100%;padding-left:0px; padding-right:0px;">
									<iframe src="../common/affectation_tapis.html"
										allowfullscreen=""
										style="height:500px; width:100%; marginwidth:0; marginheight:0"
										frameborder="0"/>
								</div>
							</div>
						</xsl:if>
					</div>
				</div>
			</div>
		</body>
	</xsl:template>

	<!-- TEMPLATES -->

	<!-- SELECTEUR DE TEMPLATE -->
	<xsl:template match="epreuve">
		<xsl:param name="type"/>

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

	<!-- TEMPLATE POUR SE PREPARE -->
	<xsl:template name="se_prepare_epreuve">
		<xsl:param name="epreuve"/>
		<div class="col-lg-6">
			<div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
				<a class="alert-link">
					<xsl:attribute name="href">
						<xsl:text>..</xsl:text>
						<xsl:value-of select="@directory"/>
						<xsl:text>/feuille_combats</xsl:text>
						<xsl:text>.html</xsl:text>
					</xsl:attribute>
					<xsl:value-of select="$epreuve/@libelle"/>
					<xsl:value-of select="$epreuve/@nom"/>
				</a>
			</div>
		</div>
	</xsl:template>

	<!-- TEMPLATE POUR AVANCEMENT -->
	<xsl:template name="avancement_epreuve">
		<xsl:param name="epreuve"/>

		<xsl:variable select="number($epreuve/@typePhase)" name="type1"/>
		<xsl:variable select="number($epreuve/@typePhase)" name="type2"/>

		<xsl:if test="count($epreuve/phases/phase[number(@typePhase) = 1]) > 0">
			<div class="col-lg-6">
				<div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
					<a class="alert-link">
						<xsl:attribute name="href">
							<xsl:text>..</xsl:text>
							<xsl:value-of select="@directory"/>
							<xsl:text>/poules_resultats</xsl:text>
							<xsl:text>.html</xsl:text>
						</xsl:attribute>
						<xsl:value-of select="$epreuve/@libelle"/>
						<xsl:value-of select="$epreuve/@nom"/>
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
							<xsl:text>..</xsl:text>
							<xsl:value-of select="@directory"/>
							<xsl:text>/tableau_competition</xsl:text>
							<xsl:text>.html</xsl:text>
						</xsl:attribute>
						<xsl:value-of select="$epreuve/@libelle"/>
						<xsl:value-of select="$epreuve/@nom"/>
						<xsl:text>&#32;Tableau</xsl:text>
					</a>
				</div>
			</div>

		</xsl:if>
	</xsl:template>

	<!-- TEMPLATE POUR CLASSEMENT -->
	<xsl:template name="classement_epreuve">
		<xsl:param name="epreuve"/>
		<div class="col-lg-6">
			<div class="alert alert-warning" role="alert" style="padding: 5px; text-align:left;">
				<a class="alert-link">
					<xsl:attribute name="href">
						<xsl:text>..</xsl:text>
						<xsl:value-of select="@directory"/>
						<xsl:text>/classement_final</xsl:text>
						<xsl:text>.html</xsl:text>
					</xsl:attribute>
					<xsl:value-of select="$epreuve/@libelle"/>
					<xsl:value-of select="$epreuve/@nom"/>
				</a>
			</div>
		</div>
	</xsl:template>

</xsl:stylesheet>
