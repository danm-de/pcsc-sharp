# development tools
MONODOCER	= monodocer
MONODOCS2HTML	= monodocs2html
MONODOC		= monodoc
MDOC		= mdoc
BUILDTOOL	= xbuild

CONFTARGET	= Release
DOCLANG		= en
DOCNAME		= PCSC

LIBNAME		= pcsc-sharp
LIBNAMESPACE	= PCSC

ASMSTR		= "AssemblyVersion\(\"[0-9]+\.[0-9]+\.[0-9]\.[0-9]\"\)"
ASMVER		= "[0-9]+\.[0-9]+\.[0-9]\.[0-9]"
ASSEMBLYVERSION	= $(shell grep -oE $(ASMSTR) pcsc-sharp/Properties/AssemblyInfo.cs | grep -oE $(ASMVER)) 

LIBFILE		= $(LIBNAME).dll
LIBCONFIGNAME	= $(LIBFILE).config
LIBMDB		= $(LIBFILE).mdb
LIBXMLFILE	= $(LIBNAME).xml
PKGCONFIGFILE	= $(LIBNAME).pc

SOURCEDIR	= pcsc-sharp
DOCDIR		= doc
HTMLDOCDIR	= $(DOCDIR)/htmldocs
BUILDDIR	= $(SOURCEDIR)/bin/$(CONFTARGET)
PKGCONFIG	= pkgconfig

PREFIX		= /usr/local
LIBDIR		= $(PREFIX)/lib/$(LIBNAME)
PKGCONFIGDIR	= $(PREFIX)/lib/$(PKGCONFIG)

BUILDFLAGS	= /p:Configuration=$(CONFTARGET)
SOLUTIONFILE	= pcsc-sharp.sln
SOURCEFILES	= $(shell find $(SOURCEDIR) -name *.cs) \
		  $(shell find $(SOURCEDIR) -name *.cproj)

all:	$(LIBFILE)

# dll
$(LIBFILE):	$(BUILDDIR)/$(LIBFILE) $(PKGCONFIGFILE) $(LIBCONFIGNAME)
$(BUILDDIR)/$(LIBFILE):	$(SOURCEFILES)
	mkdir -p $(BUILDDIR)
	$(BUILDTOOL) $(SOLUTIONFILE) $(BUILDFLAGS)

# pkgconfig
$(PKGCONFIGFILE):	$(BUILDDIR)/$(PKGCONFIGFILE)
$(BUILDDIR)/$(PKGCONFIGFILE):	$(SOURCEDIR)/$(PKGCONFIG)/$(PKGCONFIGFILE)
	sed     -e "s|@VERSION@|$(ASSEMBLYVERSION)|" \
		-e "s|@LIBDIR@|$(LIBDIR)|" \
			$(SOURCEDIR)/$(PKGCONFIG)/$(PKGCONFIGFILE) > \
			$(BUILDDIR)/$(PKGCONFIGFILE)

# config file
$(LIBCONFIGNAME):	$(BUILDDIR)/$(LIBCONFIGNAME)
$(BUILDDIR)/$(LIBCONFIGNAME):	$(SOURCEDIR)/$(LIBCONFIGNAME)
	cp $(SOURCEDIR)/$(LIBCONFIGNAME) \
		$(BUILDDIR)/$(LIBCONFIGNAME)

$(BUILDDIR)/$(LIBXMLFILE):	$(BUILDDIR)/$(LIBFILE)

# documentation XML files
xmldoc:	$(DOCDIR)/$(DOCLANG)/index.xml

$(DOCDIR)/$(DOCLANG)/index.xml:	$(BUILDDIR)/$(LIBFILE) $(BUILDDIR)/$(LIBXMLFILE)
	mkdir -p $(DOCDIR)/$(DOCLANG)
	$(MONODOCER) -assembly:$(BUILDDIR)/$(LIBFILE) \
		-path:$(DOCDIR)/$(DOCLANG) \
		-importslashdoc:$(BUILDDIR)/$(LIBXMLFILE) \
		-pretty \
		--delete \
		-name:$(DOCNAME)

# HTML docs
htmldoc:	$(DOCDIR)/$(HTMLDOCDIR)/index.html
$(DOCDIR)/$(HTMLDOCDIR)/index.html:	xmldoc
	$(MONODOCS2HTML) $(DOCDIR)/$(DOCLANG) \
		-o $(HTMLDOCDIR)

# edit documentation
editdoc: xmldoc
	$(MONODOC) --edit $(DOCDIR)/$(DOCLANG)

clean:
	rm -f $(BUILDDIR)/*
