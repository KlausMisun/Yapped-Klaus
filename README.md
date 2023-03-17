# Yapped - Klaus

My version of Yapped re-created with WPF 

## Why?

I wanted to mod a few things in sekiro but I wasn't able to use DsMapStudio as I am using an Intel integrated graphics so I took to using Yapped Rune Bear but there were multiple errors continously occuring so I came around to try and re-create the project myself.

I am heavily copying Yapped Rune Bear and using a few utilities found in the source code of DsMapStudio. For example the Param Class is ported from there.

## What to expect?

Nothing Much, this project is very much a personal project I am working on to not only learn how a few facets of modding Souls games work but improve my own coding skills.

## What are the current plans?

For now finishing the porting of all the features Rune Bear offers which honestly are impressively made.

Split The Code and credit people properly. (I am not too sure how to do this).

Clean up the README and make it actually make sense with proper segments

## Changes

The SoulsFormats has updated dependencies, this so far causes me no issues as they seem to be compatible but may be different for other. Check the original repo on JKAnderson's account to download it. It's a huge backbone for all the modding community!

I modified a few elements in some of the classes right and left not out of necessity but out of curiousity while learning a few things and VS22 complaining about not so important things

The Logic is Handled in an MVVM format to hopefully allow future porting, currently it is all mashed up into a single project but they are separate enough to be able to pull the ViewModels and the Views into two different Projects

## Libraries

- SoulsFormat and Its Dependencies
- Param and StridedByteArray from DSMapStudio
- Community Toolkit MVVM

## [W.I.P]




