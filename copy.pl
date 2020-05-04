use strict;
use warnings;
use Win32::Clipboard;
use Encode qw/encode decode/;

my %library;
my %classToFilename;
my %filenameToUsedLib;

my @folders = ("lib");
while(@folders > 0) {
  my $dir = pop @folders;
  opendir my $dh, $dir or die;
  while(my $libfile = readdir $dh) {
    my $path = "$dir\\$libfile";
    if($libfile eq "." or $libfile eq "..") {
      next;
    }
    elsif(-d $path) {
      push @folders, $path;
    }
    elsif($libfile =~ /\.cs$/) {
      open my $libfh, "<$path";
      my $libstr = "";
      my $copyStarted = 0;
      while(my $line = decode('UTF-8', <$libfh>)) {
        if($copyStarted == 1) {
          if($line =~ m|////end|) {
            last;
          }
          if($line =~ /(class|struct) LIB_([A-Za-z0-9_]+)/) {
            $classToFilename{$2} = $path;
          }
          if($line =~ /LIB_([A-Za-z0-9_]+)/) {
            ${$filenameToUsedLib{$path}}[@{$filenameToUsedLib{$path}}] = $1;
          }
          $libstr .= $line;
        }elsif($line =~ m|////start|) {
          $copyStarted = 1;
        }
      }
      close $libfh;
      $library{$path} = $libstr;
    }
  }
  closedir $dh;
}

my $filename = $ARGV[0];
open my $fh, "<$filename";
my $str = "";
my %usedLib;
while(my $line = decode('UTF-8', <$fh>)) {
  $str .= $line;
  while($line =~ /LIB_([A-Za-z0-9_]+)/g) {
    my $libFileName = $classToFilename{$1};
    $usedLib{$libFileName} = 1;
  }
}
close $fh;

my $changed = 1;
while($changed == 1) {
  $changed = 0;
  foreach my $item (keys(%usedLib)) {
    foreach my $item2 (@{$filenameToUsedLib{$item}}) {
      if(! defined $usedLib{$classToFilename{$item2}}) {
        $changed = 1;
        $usedLib{$classToFilename{$item2}} = 1;
      }
    }
  }
}

$str .= "namespace Library {\r\n";
foreach my $item (keys(%usedLib)) {
  $str .= $library{$item};
}
$str .= "}\r\n";

Win32::Clipboard()->Set(encode('Shift_JIS', $str));

