# development tools
MONODOCER	= monodocer
MONODOCS2HTML	= monodocs2html
MDASSEMBLER	= mdassembler
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

# documentation XML files
xmldoc:	$(DOCDIR)/$(DOCLANG)/index.xml

$(DOCDIR)/$(DOCLANG)/index.xml:	$(BUILDDIR)/$(LIBFILE)	
	mkdir -p $(DOCDIR)/$(DOCLANG)
	$(MONODOCER) -assembly:$(BUILDDIR)/$(LIBFILE) \
		-path:$(DOCDIR)/$(DOCLANG) \
		-pretty \
		-name:$(DOCNAME)

# HTML docs
htmldoc:	$(DOCDIR)/$(HTMLDOCDIR)/index.html
$(DOCDIR)/$(HTMLDOCDIR)/index.html:	xmldoc
	$(MONODOCS2HTML) $(DOCDIR)/$(DOCLANG) \
		-o $(HTMLDOCDIR)

clean:
	rm -f $(BUILDDIR)/*
