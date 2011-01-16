# pcsc-sharp - PC/SC .NET bindings
# Copyright (C) 2010 Daniel Mueller <daniel@danm.de>
#
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or (at your option) any later version.
#
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
#
# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, 
# MA  02110-1301  US
#

# development tools
GMCS		= gmcs
MONODOCER	= monodocer
MONODOC		= monodoc
MONODOCS2HTML	= monodocs2html
MDASSEMBLER	= mdassembler
MDOC		= mdoc

DEFINES		= -define:TRACE -define:DEBUG
FLAGS		= -noconfig -codepage:utf8 -warn:4 -optimize+ -debug
TARGET		= -target:library
NAMESPACES	= -r:System -r:System.Data -r:System.Xml
GMCSOPTS	= $(FLAGS) $(DEFINES) $(TARGET) $(NAMESPACES)
CONFTARGET	= Release
DOCLANG		= en
DOCNAME		= PCSC

LIBNAME		= pcsc-sharp
LIBNAMESPACE	= PCSC
ASSEMBLYVERSION	= 1.0.0.0

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

SOURCEFILES	= $(wildcard $(SOURCEDIR)/*.cs \
			$(SOURCEDIR)/Properties/*.cs \
			$(SOURCEDIR)/Exceptions/*.cs) \
			$(SOURCEDIR)/Interop/*.cs \
			$(SOURCEDIR)/Interop/Platform/*.cs \
			$(SOURCEDIR)/Iso7816/*.cs \
			$(SOURCEDIR)/Iso7816/Exceptions/*.cs \
			$(SOURCEDIR)/Iso8825/*.cs \
			$(SOURCEDIR)/Iso8825/Exceptions/*.cs \
			$(SOURCEDIR)/Iso8825/Asn1/*.cs \
			$(SOURCEDIR)/Iso8825/Asn1/Exceptions/*.cs \
			$(SOURCEDIR)/Iso8825/BasicEncodingRules/*.cs 


all:	$(LIBFILE)

# dll
$(LIBFILE):	$(BUILDDIR)/$(LIBFILE) $(PKGCONFIGFILE) $(LIBCONFIGNAME)
$(BUILDDIR)/$(LIBFILE):	$(SOURCEFILES)
	mkdir -p $(BUILDDIR)
	$(GMCS) $(GMCSOPTS) $(SOURCEFILES) \
		-out:$(BUILDDIR)/$(LIBFILE)

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

# edit documentation
editdoc:	xmldoc	
	# DEBIAN BUG
	#mv $(DOCDIR)/$(DOCLANG)/ns-$(LIBNAMESPACE).xml \
	#	$(DOCDIR)/$(DOCLANG)/$(LIBNAMESPACE).xml
	$(MONODOC) --edit $(DOCDIR)/$(DOCLANG)
	# DEBIAN BUG
	#mv $(DOCDIR)/$(DOCLANG)/$(LIBNAMESPACE).xml \
	#	$(DOCDIR)/$(DOCLANG)/ns-$(LIBNAMESPACE).xml

# HTML docs
htmldoc:	$(DOCDIR)/$(HTMLDOCDIR)/index.html
$(DOCDIR)/$(HTMLDOCDIR)/index.html:	xmldoc
	$(MONODOCS2HTML) $(DOCDIR)/$(DOCLANG) \
		-o $(HTMLDOCDIR)

# Visual Studio documentation XML file(s)
msxdoc:		$(LIBNAME).xml

$(LIBNAME).xml: $(DOCDIR)/$(DOCLANG)/index.xml
	$(MDOC) export-msxdoc $(DOCDIR)/$(DOCLANG)

clean:
	rm -f $(BUILDDIR)/*
