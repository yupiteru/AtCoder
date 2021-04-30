use strict;
use warnings;
use Win32::Clipboard;
use WWW::Mechanize;
use Time::HiRes 'sleep';
use utf8;
use Time::HiRes qw( usleep gettimeofday );
use Socket;
use IO::Handle;
use Encode qw/decode/;
binmode STDOUT, ':encoding(cp932)';

open my $propertyFh, "setupContestProperty.txt";
my $loginId = <$propertyFh>;
my $password = <$propertyFh>;
$loginId =~ s/[\r\n]+//g;
$password =~ s/[\r\n]+//g;
close $propertyFh;

my $contestId = "notinput";
my $isServerMode = 0;
if(@ARGV == 2) {
  $contestId = $ARGV[0];
  $isServerMode = 1;
}elsif(@ARGV == 1) {
  $contestId = $ARGV[0];
}else {
  print "\t>setupContest.pl ContestID [any]\n";
  print "\tas submit server mode if input any\n";
  exit;
}
$contestId =~ s/[\r\n]+//;

$ENV{'PERL_LWP_SSL_VERIFY_HOSTNAME'} = 0;

my $mech = WWW::Mechanize->new( agent => 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0 Waterfox/56.3') ;
$mech->timeout(120);

$mech->get("https://atcoder.jp/login");
$mech->submit_form(
  form_number => 2,
  fields      => {
    username    => $loginId,
    password    => $password,
  }
);

$mech->get("https://atcoder.jp/contests/$contestId");
my $contestStartDatetime = "";
my $contestEndDatetime = "";
if($mech->content() =~ /<time class='fixtime fixtime-full'>(\d+-\d+-\d+ \d+:\d+:\d+)\+.+<\/time><\/a>.+<time class='fixtime fixtime-full'>(\d+-\d+-\d+ \d+:\d+:\d+)\+.+<\/time><\/a>/s) {
  $contestStartDatetime = $1;
  $contestEndDatetime = $2;
}

print "コンテストID：$contestId\n";
print "開始時刻：$contestStartDatetime\n";
print "待機中...\n";
while(1) {
  my ($sec, $min, $hour, $mday, $mon, $year, $wday, $yday, $isdst) = localtime;
  $year += 1900;
  $mon += 1;
  my $nowTime = sprintf("%04d", $year) . "-" . sprintf("%02d", $mon) . "-" . sprintf("%02d", $mday) . " " . sprintf("%02d", $hour) . ":" . sprintf("%02d", $min) . ":" . sprintf("%02d", $sec);
  if($nowTime ge $contestStartDatetime) {
    last;
  }
  sleep 1;
}
print "開始\n";
sleep 1;

$mech->get("https://atcoder.jp/contests/$contestId/tasks");
my $problemsPage = $mech->content();
$problemsPage =~ s/[\r\n\t ]//g;
my @problemURLs = $problemsPage =~ /<td><ahref="(\/contests\/${contestId}\/tasks\/${contestId}_[a-zA-Z]+)">.+?<\/a><\/td>/g;

print "問題数：" . (@problemURLs) . "\n";

my $baseFolder = "problems";
my $isAlpha = 1;
if(@problemURLs > 26) {
  $isAlpha = 0;
}
if($isAlpha == 1) {
  system ".\\setup.pl " . chr(64 + @problemURLs);
}else {
  system ".\\setup.pl " . @problemURLs;
}

my @socketToChild;
for(my $j = 0;$j < @problemURLs; ++$j) {
  my $url = $problemURLs[$j];
  socketpair(my $hToChild, my $hFromServer, AF_UNIX, SOCK_STREAM, PF_UNSPEC);
  $hToChild->autoflush(1);
  $hFromServer->autoflush(1);
  $socketToChild[$j] = $hToChild;
  if(fork == 0) {
    $mech->get("https://atcoder.jp$url");
    my $problemNumberStr = "";
    if($isAlpha == 1) {
      $problemNumberStr = chr(65 + $j);
    }else {
      $problemNumberStr = sprintf("%03d", $j + 1);
    }
    open my $fh, ">$baseFolder/Problem${problemNumberStr}_TestCase.txt";
    my $problemContent = $mech->content();
    for(my $i = 1; ; ++$i) {
      if($problemContent =~ /<h3>入力例 $i<\/h3>\s*<pre>(.*?)<\/pre>/s) {
        my $input = $1;
        $input =~ s/[\r\n]+/\n/g;
        if($problemContent =~ /<h3>出力例 $i<\/h3>\s*<pre>(.*?)<\/pre>/s) {
          my $output = $1;
          $output =~ s/[\r\n]+/\n/g;
          print $fh "[case $i]\n";
          print $fh "$input\n";
          print $fh "[output]\n";
          print $fh "$output\n";
        }else {
          print "error : sample num-$i, url-$url\n";
        }
      }else {
        last;
      }
    }
    close $fh;
    
    if($isServerMode == 1) {
      while(1) {
        my $line = <$hFromServer>;
        $line =~ s/[\r\n]+//;
        if($line eq "end") {
          last;
        }
        $mech->submit_form(
          form_number => 2,
          fields      => {
            'data.LanguageId' => '4010',
            'sourceCode'      => decode('Shift_JIS', Win32::Clipboard()->Get()),
          }
        );
        $mech->get("https://atcoder.jp$url");
      }
    }
    exit;
  }
}

if($isServerMode == 1) {
  if(fork == 0) {
    sleep 1;
    while(1) {
      my ($sec, $min, $hour, $mday, $mon, $year, $wday, $yday, $isdst) = localtime;
      $year += 1900;
      $mon += 1;
      my $nowTime = sprintf("%04d", $year) . "-" . sprintf("%02d", $mon) . "-" . sprintf("%02d", $mday) . " " . sprintf("%02d", $hour) . ":" . sprintf("%02d", $min) . ":" . sprintf("%02d", $sec);
      if($nowTime ge $contestEndDatetime) {
        last;
      }
      sleep 1;
    }
    open my $hSubmit, ">>submit.txt";
    print $hSubmit "end\n";
    close $hSubmit;
    exit;
  }
  system "echo reset> submit.txt";
  open my $hSubmit, "<submit.txt";
  while(1) {
    my $line = <$hSubmit>;
    if(!defined $line) {
      usleep 10;
      next;
    }
    $line =~ s/[\r\n]+//;
    if($line eq "reset") {
      next;
    }
    if($line eq "end") {
      foreach my $item (@socketToChild) {
        print $item "end\n";
      }
      last;
    }
    system "copy.pl $line";
    if($isAlpha == 1) {
      if($line =~ /Problem([A-Z])\.cs/) {
        my $problemIdx = ord($1) - ord("A");
        my $handle = $socketToChild[$problemIdx];
        print $handle "go\n";
      }
    }else {
      if($line =~ /Problem([0-9]+)\.cs/) {
        my $problemIdx = $1 - 1;
        my $handle = $socketToChild[$problemIdx];
        print $handle "go\n";
      }
    }
  }
  close $hSubmit;
}

while(wait > 0){}
print "END.\n";

